using System;
using System.Collections.Generic;
using System.Text;

namespace RemoveSilence
{
    class Program
    {

        static void Main(string[] args)
        {
            Works();
            //TryThat();
            //TryThis();
        }

        private static void Works()
        {
            string DEFAULTDIRECTORY = "C:\\Users\\Zachary Burns\\Desktop\\TEMPDIR\\";
            Un4seen.Bass.BassNet.Registration("bass@zackburns.com", "2X113281839322");
            Un4seen.Bass.BassNet.OmitCheckVersion = true;
            string INPUTFILE = "test.mp3";
            int _inputfilestream;

            float currentlevel = 0f;            
            float maxLevelLeft = 0f;
            float maxLevelRight = 0f;
            float peakLevelLeft = 0f;
            float peakLevelRight = 0f;
            float DBTHRESHOLD = -40f;
            
            //CURRENT DEFAULTS
            ShowCurrentDefaults();

            //Get Party Started
            bool bassinit = Un4seen.Bass.Bass.BASS_Init(0, 22050, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero);
            if (bassinit == true)
            {
                if (System.IO.File.Exists(DEFAULTDIRECTORY + INPUTFILE))
                {
                    _inputfilestream = Un4seen.Bass.Bass.BASS_StreamCreateFile(DEFAULTDIRECTORY + INPUTFILE, 0, 0, Un4seen.Bass.BASSFlag.BASS_STREAM_DECODE | Un4seen.Bass.BASSFlag.BASS_STREAM_PRESCAN | Un4seen.Bass.BASSFlag.BASS_SAMPLE_FLOAT);
                    if (_inputfilestream != 0)
                    {
                        int bytesread;
                        Un4seen.Bass.AddOn.Enc.BassEnc.BASS_Encode_Start(_inputfilestream, DEFAULTDIRECTORY + "output.wav", Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_FP_32BIT | Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_AUTOFREE | Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_PCM | Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_PAUSE, null, System.IntPtr.Zero);
                        //Un4seen.Bass.Misc.WaveWriter wav = new Un4seen.Bass.Misc.WaveWriter(DEFAULTDIRECTORY + "output.wav", 1, 22050, 32, true);
                        while (Un4seen.Bass.Bass.BASS_ChannelIsActive(_inputfilestream) == Un4seen.Bass.BASSActive.BASS_ACTIVE_PLAYING)
                        {
                            int LenthInBytes = (int)Un4seen.Bass.Bass.BASS_ChannelSeconds2Bytes(_inputfilestream, 0.02);
                            int FloatsRequired = LenthInBytes / 4; //32-bits = 4 bytes
                            float[] samplebuffer = new float[FloatsRequired];
                            bytesread = Un4seen.Bass.Bass.BASS_ChannelGetData(_inputfilestream, samplebuffer, LenthInBytes);
                            Console.SetCursorPosition(0, 3);
                            Console.Write("File Position: " + Math.Round((double)(Un4seen.Bass.Bass.BASS_ChannelGetPosition(_inputfilestream, Un4seen.Bass.BASSMode.BASS_POS_DECODE) / 1000000),2) + "MB");
                            FloatsRequired = bytesread / 4;  //number of 32 bit floats returned

                            bool written = false;

                            for (int i = 0; i < FloatsRequired; i++)
                            {
                                currentlevel = Math.Abs(samplebuffer[i]);
                                
                                //write if needed
                                if (Math.Round(Un4seen.Bass.Utils.LevelToDB((int)Math.Round(32767f * currentlevel) & 0xFFFF, 65535), 2) > DBTHRESHOLD && !written)
                                {
                                    written = Un4seen.Bass.AddOn.Enc.BassEnc.BASS_Encode_Write(_inputfilestream, samplebuffer, bytesread);
                                }

                                if (i % 2 == 0) //working left channel float data
                                {
                                    if (currentlevel > maxLevelLeft)
                                        maxLevelLeft = currentlevel;
                                    //Console.SetCursorPosition(0, 0);
                                    //Console.Write("Curr db L :" + Math.Round(Un4seen.Bass.Utils.LevelToDB((int)Math.Round(32767f * currentlevel) & 0xFFFF, 65535),2) + "    ");
                                }
                                else
                                {
                                    if (currentlevel > maxLevelRight)
                                        maxLevelRight = currentlevel;
                                    //Console.SetCursorPosition(0, 1);
                                    //Console.Write("Curr db R :" + Math.Round(Un4seen.Bass.Utils.LevelToDB((int)Math.Round(32767f * currentlevel) & 0xFFFF, 65535), 2) + "    ");
                                }
                            }
                            // limit the maximum peak levels to +6bB = 65535 = 0xFFFF
                            // the peak levels will be int values, where 32767 = 0dB
                            // and a float value of 1.0 also represents 0db.
                            peakLevelLeft = (int)Math.Round(32767f * maxLevelLeft) & 0xFFFF;
                            peakLevelRight = (int)Math.Round(32767f * maxLevelRight) & 0xFFFF;
                            Console.SetCursorPosition(25, 0);
                            Console.Write("Peak db L :" + Math.Round(Un4seen.Bass.Utils.LevelToDB(peakLevelLeft, 65535),2) + "        ");
                            Console.SetCursorPosition(25, 1);
                            Console.Write("Peak db R :" + Math.Round(Un4seen.Bass.Utils.LevelToDB(peakLevelRight, 65535),2) + "        ");
                        }
                        Un4seen.Bass.AddOn.Enc.BassEnc.BASS_Encode_Stop(_inputfilestream);
                    }
                    else
                    {
                        Console.WriteLine(Un4seen.Bass.Bass.BASS_ErrorGetCode().ToString());
                    }
                    Un4seen.Bass.Bass.BASS_StreamFree(_inputfilestream);
                }
            }
            Un4seen.Bass.Bass.BASS_Free();
        }

        private static void ShowCurrentDefaults()
        {
            Console.WriteLine("BASS_CONFIG_FLOATDSP: " + Un4seen.Bass.Bass.BASS_GetConfig(Un4seen.Bass.BASSConfig.BASS_CONFIG_FLOATDSP));
        }


        private static void TryThat()
        {
            string DEFAULTDIRECTORY = "C:\\Users\\Zachary Burns\\Desktop\\TEMPDIR";

            System.IO.FileStream fs = System.IO.File.OpenRead(DEFAULTDIRECTORY + "\\test.mp3");
            int length = (int)fs.Length;
            byte[] buffer = new byte[length];
            fs.Read(buffer, 0, length);
            fs.Close();

            Un4seen.Bass.BassNet.Registration("bass@zackburns.com", "2X113281839322");
            Un4seen.Bass.BassNet.OmitCheckVersion = true;
            if (Un4seen.Bass.Bass.BASS_Init(-1, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                System.Runtime.InteropServices.GCHandle gch = System.Runtime.InteropServices.GCHandle.Alloc(buffer, System.Runtime.InteropServices.GCHandleType.Pinned);
                int stream = Un4seen.Bass.Bass.BASS_StreamCreateFile(gch.AddrOfPinnedObject(), 0L, buffer.Length, Un4seen.Bass.BASSFlag.BASS_SAMPLE_FLOAT);
                Un4seen.Bass.AddOn.Enc.BassEnc.BASS_Encode_Start(stream, DEFAULTDIRECTORY + "\\test" + ".wav", Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_PCM, null, IntPtr.Zero);
                Un4seen.Bass.Bass.BASS_ChannelPlay(stream, false);
                Un4seen.Bass.Bass.BASS_StreamFree(stream);
                Un4seen.Bass.Bass.BASS_Free();
                gch.Free();
            }
        }


        private static void TryThis()
        {
            int _inputfilestream = 0;
            int bufferlength = 65535;
            int LEVELMIN = 33;
            string DEFAULTDIRECTORY = "C:\\Users\\Zachary Burns\\Desktop\\TEMPDIR";

            if (!System.IO.Directory.Exists(DEFAULTDIRECTORY))
                System.IO.Directory.CreateDirectory(DEFAULTDIRECTORY);

            foreach (string filename in System.IO.Directory.GetFiles(DEFAULTDIRECTORY, "*.mp3"))
            {

                Console.WriteLine("Processing " + filename + " at " + LEVELMIN);
                Un4seen.Bass.BassNet.Registration("bass@zackburns.com", "2X113281839322");
                Un4seen.Bass.BassNet.OmitCheckVersion = true;
                bool bassinit = Un4seen.Bass.Bass.BASS_Init(0, 22050, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, System.IntPtr.Zero);
                if (bassinit == true)
                {
                    if (System.IO.File.Exists(filename))
                    {
                        _inputfilestream = Un4seen.Bass.Bass.BASS_StreamCreateFile(filename, 0, 0, Un4seen.Bass.BASSFlag.BASS_STREAM_DECODE | Un4seen.Bass.BASSFlag.BASS_STREAM_PRESCAN | Un4seen.Bass.BASSFlag.BASS_SAMPLE_FLOAT);
                        Console.SetCursorPosition(0, 0);
                        Console.WriteLine("Total Length\t" + Un4seen.Bass.Bass.BASS_ChannelGetLength(_inputfilestream));
                        if (_inputfilestream != 0)
                        {
                            float[] buff = new float[bufferlength];
                            int bytesread;
                            Un4seen.Bass.AddOn.Enc.BassEnc.BASS_Encode_Start(_inputfilestream, filename + ".wav", Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_FP_32BIT | Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_AUTOFREE | Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_PCM | Un4seen.Bass.AddOn.Enc.BASSEncode.BASS_ENCODE_PAUSE, null, System.IntPtr.Zero);
                            while (Un4seen.Bass.Bass.BASS_ChannelIsActive(_inputfilestream) == Un4seen.Bass.BASSActive.BASS_ACTIVE_PLAYING)
                            {
                                bytesread = Un4seen.Bass.Bass.BASS_ChannelGetData(_inputfilestream, buff, buff.Length);
                                Console.SetCursorPosition(0, 1);
                                Console.WriteLine("At Position\t" + Un4seen.Bass.Bass.BASS_ChannelGetPosition(_inputfilestream) + "\t\tLevel: " + Un4seen.Bass.Utils.LowWord(Un4seen.Bass.Bass.BASS_ChannelGetLevel(_inputfilestream)) + "\t");
                                if (Un4seen.Bass.Utils.LowWord(Un4seen.Bass.Bass.BASS_ChannelGetLevel(_inputfilestream)) > LEVELMIN)
                                {
                                    Un4seen.Bass.AddOn.Enc.BassEnc.BASS_Encode_Write(_inputfilestream, buff, bytesread);
                                }
                            }
                            Un4seen.Bass.AddOn.Enc.BassEnc.BASS_Encode_Stop(_inputfilestream);
                        }
                        else
                        {
                            Console.WriteLine(Un4seen.Bass.Bass.BASS_ErrorGetCode().ToString());
                        }
                        Un4seen.Bass.Bass.BASS_StreamFree(_inputfilestream);
                    }
                    else
                    {
                        Console.WriteLine(filename + "\" is missing");
                    }
                }
                else
                {
                    Console.WriteLine("Error Initializing BASS.NET Library");
                }
                // free BASS
                Un4seen.Bass.Bass.BASS_Free();
            }
        }
    }
}
