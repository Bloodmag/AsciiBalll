using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace ConsoleWindowRenderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
        public short X;
        public short Y;

        public Coord(short X, short Y)
        {
            this.X = X;
            this.Y = Y;
        }
    };

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharUnion
    {
        [FieldOffset(0)] public char UnicodeChar;
        [FieldOffset(0)] public byte AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct CharInfo
    {
        [FieldOffset(0)] public CharUnion Char;
        [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }
    static class W
    {

        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteConsoleOutputW(
          SafeFileHandle hConsoleOutput,
          CharInfo[] lpBuffer,
          Coord dwBufferSize,
          Coord dwBufferCoord,
          ref SmallRect lpWriteRegion);

        [DllImport("Kernel32.dll", SetLastError = true)]
        static extern bool SetConsoleCP(uint wCodePageID);

        

        private static readonly SafeFileHandle h;
        private static CharInfo[] buf;
        private static SmallRect rect;// new SmallRect() { Left = 0, Top = 0, Right = 120, Bottom = 40 };
        private static int width;
        private static int height;

        public static CharInfo[] Buf { get => buf; }

        static W()
        {
            h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);
            if (h.IsInvalid)
            {
                throw new ExternalException("Unable to get console buffer access!");
            }
        }

        public static bool SetupBuffer(int _width, int _height)
        {
            width = _width;
            height = _height;
            rect = new SmallRect() { Left = 0, Top = 0, Right = (short)width, Bottom = (short)height };
            buf = new CharInfo[width*height];
            Console.SetWindowSize(width, height);
            Console.OutputEncoding = Encoding.Unicode;

            return true;
        }

        public static async void DrawAsync()
        {
            await Task.Run(() => Draw());
        }
        public static void  Draw()
        {
            WriteConsoleOutputW(h, Buf,
                              new Coord() { X = (short)width, Y = (short)height },
                              new Coord() { X = 0, Y = 0 },
                              ref rect);
        }

    }
}

