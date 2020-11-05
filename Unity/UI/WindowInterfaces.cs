
public interface IWindowBehaviour
{
    //Used as ID
}

public interface IOnWindowOpen : IWindowBehaviour
{
    void OnWindowOpen();
}

public interface IOnWindowClose : IWindowBehaviour
{
    void OnWindowClose();
}

public interface IOnWindowFocus : IWindowBehaviour
{
    void OnWindowFocus();
}

public interface IOnWindowUnfocus : IWindowBehaviour
{
    void OnWindowUnfocus();
}

public interface IOnWindowBackPressed : IWindowBehaviour
{
    void OnWindowBackPressed();
}