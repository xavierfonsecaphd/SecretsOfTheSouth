
public interface IOGBaseTask
{
    void Update();
    bool IsCompleted();
    bool IsFailed();
    bool IsSucceeded();
    OGFormattedLocalizedText GetDescription();
    OGTaskResult Result { get; }
}