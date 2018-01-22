namespace Phoenix.Devices.Printers.DatecsMD
{
    using System;

    public enum Flag : byte
    {
        Begin = 1,
        End = 3,
        EndData = 5,
        NAK = 0x15,
        Response1 = 0x31,
        Response2 = 50,
        Response3 = 0x33,
        ResponseD = 0x44,
        ResponseE = 0x45,
        ResponseF = 70,
        ResponseI = 0x49,
        ResponseOK = 80,
        ResponseR = 0x52,
        Separator = 4,
        SYN = 0x16
    }
}

