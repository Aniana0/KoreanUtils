using System;
using KoreanUtils;

class Program {
    static void Main()
    {
        Console.WriteLine(KoreanUtils.KoreanManuallyG2P.CharG2p('읗',"히히", "이란"));
        Console.WriteLine(KoreanUtils.KoreanManuallyG2P.CharG2p('란',"임진란임진란임진", "진란"));
    }
}
