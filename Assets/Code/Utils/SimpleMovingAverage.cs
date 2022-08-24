/// <summary>
/// Tracks a moving average
/// Taken from stack overflow, lost the specific link
/// </summary>
public class SimpleMovingAverage
{
    private readonly int _k;
    private readonly float[] _values;

    private int _index = 0;
    private float _sum = 0;

    public SimpleMovingAverage(int k)
    {
        if (k <= 0) throw new System.ArgumentOutOfRangeException(nameof(k), "Must be greater than 0");

        _k = k;
        _values = new float[k];
    }

    public float Update(float nextInput)
    {
        // calculate the new sum
        _sum = _sum - _values[_index] + nextInput;

        // overwrite the old value with the new one
        _values[_index] = nextInput;

        // increment the index (wrapping back to 0)
        _index = (_index + 1) % _k;

        // calculate the average
        return ((float)_sum) / _k;
    }
}
