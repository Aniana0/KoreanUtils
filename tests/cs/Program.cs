using System;
using KoreanUtils;

class Program {
    static void Main()
    {
        Console.WriteLine(KoreanUtils.KoreanManuallyG2P.CharG2p('슉',"슉슉", "슉슉"));
        Console.WriteLine(KoreanUtils.KoreanManuallyG2P.CharG2p('감',"난", "자"));
    }
}
