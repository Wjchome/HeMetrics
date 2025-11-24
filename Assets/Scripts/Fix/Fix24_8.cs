using UnityEngine;

public struct Fixed24_8
{
    // 位划分常量：小数部分占8位
    public const int FractionBits = 8;
    public const int Scale = 1 << FractionBits; // 2^8 = 256（缩放因子）
    private int _value; // 存储定点数的原始int值

    // 私有构造函数：直接从原始int值初始化（内部运算用）
    private Fixed24_8(int rawValue)
    {
        _value = rawValue;
    }

    // 从float转换为定点数
    public static Fixed24_8 FromFloat(float value)
    {
        return new Fixed24_8(Mathf.RoundToInt(value * Scale));
    }

    // 转换为float（仅用于显示）
    public float ToFloat()
    {
        return (float)_value / Scale;
    }

    // 从整数直接转换（整数部分直接左移8位，小数部分为0）
    public static Fixed24_8 FromInt(int integer)
    {
        return new Fixed24_8(integer << FractionBits);
    }

    // 提取整数部分（向下取整）
    public int GetIntegerPart()
    {
        return _value >> FractionBits; // 右移8位，丢弃小数部分
    }

    // 提取小数部分（0~255，对应0~1-1/256）
    public int GetFractionPart()
    {
        return _value & 0xFF; // 与运算保留低8位
    }

    // 加法：使用long避免溢出
    public static Fixed24_8 operator +(Fixed24_8 a, Fixed24_8 b)
    {
        long result = (long)a._value + b._value;
        // 可选：添加溢出检查
        if (result > int.MaxValue || result < int.MinValue)
        {
            Debug.LogWarning("Fixed24_8 addition overflow!");
        }
        return new Fixed24_8((int)result);
    }

    // 减法：使用long避免溢出
    public static Fixed24_8 operator -(Fixed24_8 a, Fixed24_8 b)
    {
        long result = (long)a._value - b._value;
        if (result > int.MaxValue || result < int.MinValue)
        {
            Debug.LogWarning("Fixed24_8 subtraction overflow!");
        }
        return new Fixed24_8((int)result);
    }

    // 一元负号运算符
    public static Fixed24_8 operator -(Fixed24_8 a)
    {
        return new Fixed24_8(-a._value);
    }

    // 乘法：结果需右移8位还原精度（避免溢出，先用long过渡）
    public static Fixed24_8 operator *(Fixed24_8 a, Fixed24_8 b)
    {
        long result = (long)a._value * b._value; // 用long避免32位int溢出
        return new Fixed24_8((int)(result >> FractionBits)); // 右移8位还原
    }

    // 与int的乘法（使用long避免溢出）
    public static Fixed24_8 operator *(Fixed24_8 a, int b)
    {
        long result = (long)a._value * b;
        if (result > int.MaxValue || result < int.MinValue)
        {
            Debug.LogWarning("Fixed24_8 integer multiplication overflow!");
        }
        return new Fixed24_8((int)result);
    }

    // 除法：先左移8位再除，保留小数精度
    public static Fixed24_8 operator /(Fixed24_8 a, Fixed24_8 b)
    {
        if (b._value == 0)
        {
            Debug.LogError("Division by zero");
            return a;
        }

        long result = (long)a._value << FractionBits; // 左移8位放大被除数
        return new Fixed24_8((int)(result / b._value));
    }

    // 比较运算符
    public static bool operator ==(Fixed24_8 a, Fixed24_8 b)
    {
        return a._value == b._value;
    }

    public static bool operator !=(Fixed24_8 a, Fixed24_8 b)
    {
        return a._value != b._value;
    }

    public static bool operator <(Fixed24_8 a, Fixed24_8 b)
    {
        return a._value < b._value;
    }

    public static bool operator >(Fixed24_8 a, Fixed24_8 b)
    {
        return a._value > b._value;
    }

    public static bool operator <=(Fixed24_8 a, Fixed24_8 b)
    {
        return a._value <= b._value;
    }

    public static bool operator >=(Fixed24_8 a, Fixed24_8 b)
    {
        return a._value >= b._value;
    }

    // 重写Equals和GetHashCode以符合值类型规范
    public override bool Equals(object obj)
    {
        return obj is Fixed24_8 other && _value == other._value;
    }

    public override int GetHashCode()
    {
        return _value.GetHashCode();
    }

    // 显式转换：从float到Fixed24_8（更安全）
    public static implicit operator Fixed24_8(float value)
    {
        return FromFloat(value);
    }

    // 显式转换：从Fixed24_8到float
    public static explicit operator float(Fixed24_8 value)
    {
        return value.ToFloat();
    }

    // 友好的字符串表示
    public override string ToString()
    {
        return ToFloat().ToString("F3"); // 保留3位小数
    }

    // 带格式的字符串表示
    public string ToString(string format)
    {
        return ToFloat().ToString(format);
    }
}