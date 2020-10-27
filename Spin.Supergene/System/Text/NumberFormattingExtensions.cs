namespace System.Text
{
  public static class NumberFormattingExtensions
  {
    public static void Test()
    {
      Console.WriteLine(1d.ToOrdinal());
      Console.WriteLine(2d.ToOrdinal());
      Console.WriteLine(3d.ToOrdinal("n0"));

      Console.WriteLine(31d.ToOrdinal());
      Console.WriteLine(32d.ToOrdinal());
      Console.WriteLine(33d.ToOrdinal("n0"));

      Console.WriteLine(3331d.ToOrdinal());
      Console.WriteLine(3332d.ToOrdinal());
      Console.WriteLine(3333d.ToOrdinal("n0"));

      Console.WriteLine(1d.ToOrdinal());
      Console.WriteLine(11d.ToOrdinal());
      Console.WriteLine(111d.ToOrdinal());
      Console.WriteLine(1111d.ToOrdinal());
      Console.WriteLine(11111d.ToOrdinal("n0"));

      Console.WriteLine(9d.ToOrdinal());
      Console.WriteLine(99d.ToOrdinal());
      Console.WriteLine(999d.ToOrdinal());
      Console.WriteLine(9999d.ToOrdinal());
      Console.WriteLine(99999d.ToOrdinal("n0"));
    }

    public static string ToOrdinal(this decimal number, string format = null) => number.ToString(format) + ToOrdinalSuffix(number);
    public static string ToOrdinal(this double number, string format = null) => number.ToString(format) + ToOrdinalSuffix(number);
    public static string ToOrdinal(this int number, string format = null) => number.ToString(format) + ToOrdinalSuffix(number);
    private static string ToOrdinalSuffix(this decimal number) => GetOrdinalSuffix((int)(number / 10 % 10), (int)(number % 10));
    private static string ToOrdinalSuffix(this double number) => GetOrdinalSuffix((int)(number / 10 % 10), (int)(number % 10));
    private static string ToOrdinalSuffix(this int number) => GetOrdinalSuffix(number / 10 % 10, number % 10);
    private static string GetOrdinalSuffix(int tens, int ones) =>
      tens == 1 ? "th" :
        (ones == 1) ? "st" :
        (ones == 2) ? "nd" :
        (ones == 3) ? "rd" :
        "th";
  }
}
