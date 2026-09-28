using System;
using System.Text;
using System.Threading;

namespace cube
{
    internal class Program
    {

       

            // Rotation angles
        static float rotationA, rotationB, rotationC;

        // Cube size (half-extent), console dimensions and buffers
        static float cubeHalfSize = 20;
        static int consoleWidth = 160, consoleHeight = 44;
        static float[] depthBuffer = new float[160 * 44];
        static char[] screenBuffer = new char[160 * 44];
        static char backgroundChar = '.';
        static int cameraDistance = 100;
        static float horizontalOffset;
        static float projectionScale = 40;

        static float step = 0.6f;

        // Temporary per-vertex transformed coordinates and projection helpers
        static float transformedX, transformedY, transformedZ;
        static float invZ;
        static int projectedX, projectedY;
        static int bufferIndex;

        static float CalculateX(float i, float j, float k)
        {
            return j * MathF.Sin(rotationA) * MathF.Sin(rotationB) * MathF.Cos(rotationC) - k * MathF.Cos(rotationA) * MathF.Sin(rotationB) * MathF.Cos(rotationC) +
                   j * MathF.Cos(rotationA) * MathF.Sin(rotationC) + k * MathF.Sin(rotationA) * MathF.Sin(rotationC) + i * MathF.Cos(rotationB) * MathF.Cos(rotationC);
        }

        static float CalculateY(float i, float j, float k)
        {
            return j * MathF.Cos(rotationA) * MathF.Cos(rotationC) + k * MathF.Sin(rotationA) * MathF.Cos(rotationC) -
                   j * MathF.Sin(rotationA) * MathF.Sin(rotationB) * MathF.Sin(rotationC) + k * MathF.Cos(rotationA) * MathF.Sin(rotationB) * MathF.Sin(rotationC) -
                   i * MathF.Cos(rotationB) * MathF.Sin(rotationC);
        }

        static float CalculateZ(float i, float j, float k)
        {
            return k * MathF.Cos(rotationA) * MathF.Cos(rotationB) - j * MathF.Sin(rotationA) * MathF.Cos(rotationB) + i * MathF.Sin(rotationB);
        }

        static void CalculateForSurface(float cubeX, float cubeY, float cubeZ, char ch)
        {
            transformedX = CalculateX(cubeX, cubeY, cubeZ);
            transformedY = CalculateY(cubeX, cubeY, cubeZ);
            transformedZ = CalculateZ(cubeX, cubeY, cubeZ) + cameraDistance;

            invZ = 1 / transformedZ;

            projectedX = (int)(consoleWidth / 2 + horizontalOffset + projectionScale * invZ * transformedX * 2);
            projectedY = (int)(consoleHeight / 2 + projectionScale * invZ * transformedY);

            bufferIndex = projectedX + projectedY * consoleWidth;
            if (bufferIndex >= 0 && bufferIndex < consoleWidth * consoleHeight)
            {
                if (invZ > depthBuffer[bufferIndex])
                {
                    depthBuffer[bufferIndex] = invZ;
                    screenBuffer[bufferIndex] = ch;
                }
            }
        }



            static void Main()
            {
                Console.WriteLine("Hello, nihilistic World!. Press Q during the animation to exit!");
                Console.ReadKey(true);
                Console.CursorVisible = false;
                Console.Clear();

                StringBuilder frameOutput = new StringBuilder(consoleWidth * consoleHeight + consoleHeight);

                while (true)
                {
                    Array.Fill(screenBuffer, backgroundChar);
                    Array.Clear(depthBuffer, 0, depthBuffer.Length);

                    // First cube
                    cubeHalfSize = 20;
                    horizontalOffset = -2 * cubeHalfSize;
                    RenderCube();

                    // Second cube
                    cubeHalfSize = 10;
                    horizontalOffset = 1 * cubeHalfSize;
                    RenderCube();

                    // Third cube
                    cubeHalfSize = 5;
                    horizontalOffset = 8 * cubeHalfSize;
                    RenderCube();

                    // Render frame to console
                    Console.SetCursorPosition(0, 0);
                    frameOutput.Clear();
                    for (int k = 0; k < consoleWidth * consoleHeight; k++)
                    {
                        frameOutput.Append(k % consoleWidth != 0 ? screenBuffer[k] : '\n');
                    }
                    Console.Write(frameOutput.ToString());

                    rotationA += 0.05f;
                    rotationB += 0.05f;
                    rotationC += 0.01f;

                    // Check for graceful termination (Q or q)
                    if (Console.KeyAvailable)
                    {
                        var keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Q)
                        {
                            break;
                        }
                    }

                    // Equivalent to usleep(16000) -> 16 milliseconds delay (~60 FPS)
                    Thread.Sleep(16);
                }
                // Restore console state after exiting loop
                Console.CursorVisible = true;
                Console.WriteLine();
            }

            static void RenderCube()
            {
                for (float cubeX = -cubeHalfSize; cubeX < cubeHalfSize; cubeX += step)
                {
                    for (float cubeY = -cubeHalfSize; cubeY < cubeHalfSize; cubeY += step)
                    {
                        CalculateForSurface(cubeX, cubeY, -cubeHalfSize, '@');
                        CalculateForSurface(cubeHalfSize, cubeY, cubeX, '$');
                        CalculateForSurface(-cubeHalfSize, cubeY, -cubeX, '~');
                        CalculateForSurface(-cubeX, cubeY, cubeHalfSize, '#');
                        CalculateForSurface(cubeX, -cubeHalfSize, -cubeY, ';');
                        CalculateForSurface(cubeX, cubeHalfSize, cubeY, '+');
                    }
                }
            }
        }
    }

