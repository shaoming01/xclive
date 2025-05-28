namespace SchemaBuilder.Svc.Helpers;

public static class DecimalExtent
{
    /// <summary>保留小数，四舍五入</summary>
    /// <param name="value"></param>
    /// <param name="n">小数位数</param>
    /// <returns></returns>
    public static Decimal Digits(this Decimal value, int n = 2)
    {
        return Decimal.Round(value, n, MidpointRounding.AwayFromZero);
    }

    /// <summary>用于金额分摊</summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="setValue">分摊后的设置值</param>
    /// <param name="rateFunc">比例</param>
    /// <param name="shareAmount">总额</param>
    /// <param name="decimalCount"></param>
    /// <param name="minVal"></param>
    public static void Share<T>(
        this IEnumerable<T> query,
        Action<T, Decimal> setValue,
        Func<T, Decimal> rateFunc,
        Decimal shareAmount,
        uint decimalCount = 2,
        Decimal minVal = 0M)
    {
        T[] array = query.ToArray<T>();
        if (array.Length == 0)
            return;
        Decimal num1 = ((IEnumerable<T>)array).Sum<T>((Func<T, Decimal>)(a => Math.Abs(rateFunc(a))));
        Decimal num2 = 0M;
        for (int index = 0; index < array.Length; ++index)
        {
            if (num1 == 0M)
            {
                setValue(array[index], 0M);
            }
            else
            {
                Decimal num3 = Math.Abs(rateFunc(array[index]));
                Decimal num4 = (shareAmount * num3 / num1).CutDecimal(decimalCount);
                if (Math.Abs(num4) <= minVal)
                    num4 = minVal;
                if (index + 1 == array.Length)
                    num4 = shareAmount - num2;
                num2 += num4;
                setValue(array[index], num4);
            }
        }
    }

    /// <summary>保留小数，不四舍五入</summary>
    /// <param name="input"></param>
    /// <param name="decimalCount"></param>
    /// <returns></returns>
    public static Decimal CutDecimal(this Decimal input, uint decimalCount = 2)
    {
        Decimal num = (Decimal)Math.Pow(10.0, (double)decimalCount);
        return (Decimal)(long)(input * num) / num;
    }

    /// <summary>去除多余的0</summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Decimal Cut0(this Decimal value) => value / 1.0000000000000000000000000000M;
}