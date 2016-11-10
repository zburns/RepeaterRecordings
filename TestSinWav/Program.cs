using System;
using System.Collections.Generic;
using System.Text;
using Un4seen.Bass;
using System.Runtime.InteropServices;

namespace TestSinWav
{
    class Program
    {
        static STREAMPROC streamCreate;
        static long bufferLength;

        const double twoPI = 6.283185;
        static int sampleRate = 44100;

        static int channel;

        static short[] data; //buffer

        static double frequency;
        static double increment;
        static double wavePosition = 0;

        static double frequency2;
        static double increment2;
        static double wavePosition2 = 0;

        static void Main(string[] args)
        {
            Un4seen.Bass.BassNet.Registration("bass@zackburns.com", "2X113281839322");
            Un4seen.Bass.BassNet.OmitCheckVersion = true;
            bool bassinit = Un4seen.Bass.Bass.BASS_Init(0, sampleRate, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero);
            if (bassinit)
            {
                streamCreate = new STREAMPROC(WaveformProc);
                channel = Bass.BASS_StreamCreate(sampleRate, 2, BASSFlag.BASS_DEFAULT, streamCreate, IntPtr.Zero);
                if (channel != 0)
                {
                    bufferLength = Bass.BASS_ChannelSeconds2Bytes(channel, (double)Bass.BASS_GetConfig(BASSConfig.BASS_CONFIG_BUFFER) / 1000d);
                    data = new short[bufferLength];

                    playSine();
                }
                else
                {
                    Un4seen.Bass.BASSError error = Un4seen.Bass.Bass.BASS_ErrorGetCode();
                    Console.WriteLine(error.ToString());
                }
            }
            else
            {
                Console.WriteLine("FAIL");
            }

            Console.ReadLine();
        }

        private static void playSine()
        {
            //# of half steps UP from A4 (use - to go down)
            int halfSteps = 5; //5 half steps up = play D and F#

            frequency = 440d * Math.Pow(2d, ((double)halfSteps / 12d));
            increment = twoPI * frequency / (double)sampleRate;

            frequency2 = 440d * Math.Pow(2d, (((double)halfSteps + 4) / 12d));
            increment2 = twoPI * frequency2 / (double)sampleRate;

            Bass.BASS_ChannelPlay(channel, false);
        }

        private static int WaveformProc(int handle, IntPtr buffer, int length, IntPtr user)
        {
            int waveCalculated = length;
            //waveCalculated |= (int)BASSStreamProc.BASS_STREAMPROC_END;

            length /= 2;

            int temporarySignalSum;

            for (int a = 0; a < length; a += 2)
            {
                temporarySignalSum = Convert.ToInt32(16383d * Math.Sin(wavePosition) + 16383d * Math.Sin(wavePosition2)); //Max total = 32767d

                //Max total amplitude = 32767
                if (temporarySignalSum > 32767)
                {
                    temporarySignalSum = 32767;
                }

                //Stereo
                data[a] = (short)temporarySignalSum;
                data[a + 1] = data[a];

                wavePosition += increment;
                wavePosition2 += increment2;

                if (wavePosition > twoPI)
                {
                    wavePosition -= twoPI;
                }

                if (wavePosition2 > twoPI)
                {
                    wavePosition2 -= twoPI;
                }
            }

            Marshal.Copy(data, 0, buffer, length);

            return waveCalculated;
        }
    }
}