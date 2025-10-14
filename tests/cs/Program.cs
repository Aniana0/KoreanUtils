using System;
using KoreanUtils;

class Program {
    static void Main()
    {
        Console.WriteLine(KoreanUtils.KoreanManuallyG2P.CharG2p('감', "", "자야"));
        Console.WriteLine(KoreanUtils.KoreanManuallyG2P.CharG2p('해', "못", "해"));
    }
}
