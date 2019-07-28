using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
 
public class ContextEditor : EditorWindow
{
    [MenuItem("RocketWorks/Context Editor")]
    public static void Open()
    {
        ContextEditor window = (ContextEditor)EditorWindow.GetWindow(typeof(ContextEditor));
        window.Show();
    }

    public void OnEnable()
    {
        var root = this.rootVisualElement;

        var nodeCanvas = new NodeCanvas();
        root.Add(nodeCanvas);
        nodeCanvas.StretchToParentSize();

        var toolbarButton = new ToolbarButton(() => Debug.Log("Hello"));
        toolbarButton.Add(new Label("Log Hello"));
        toolbarButton.style.width = 200f;
        toolbarButton.style.height = 30f;
        root.Add(toolbarButton);
    }
}

public class GameContextData
{
    public List<ContextData> contexts;
    public List<ContextData> componentTypes;
}

public class ContextData
{
    public string name;
    public List<string> componentNames;
}

public class ComponentData
{
    public string name;
}

public class FieldData
{
    
}

public class NodeCanvas : VisualElement
{
    private VisualElement menu;

    public NodeCanvas() : base()
    {
        menu = new RightClickMenu();
    }

    override public void HandleEvent(EventBase evt)
    {
        if(evt is PointerUpEvent pointerUp)
        {
            if(this.Contains(menu))
            {
                this.Remove(menu);
                return;
            }
            this.Add(menu);
            menu.style.left = pointerUp.localPosition.x;
            menu.style.top = pointerUp.localPosition.y;
        }
    }
}

public class RightClickMenu : VisualElement
{
    private Type[] options = {typeof(ComponentData), typeof(ContextData)};

    public RightClickMenu()
    {
        
        this.style.width = 200f;
        this.style.paddingBottom = 3f;
        this.style.paddingTop = 3f;
        this.style.backgroundColor = Color.grey * .2f;
        this.style.position = Position.Absolute;

        this.CaptureMouse();

        foreach(var option in options)
        {
            var button = new Button(() => SelectOption(option));
            button.style.height = 20f;
            button.Add(new Label($"Create {option.Name}"));
            Add(button);
        }
    }

    private void SelectOption(Type option)
    {
        Debug.Log(option);
        Node node = Activator.CreateInstance(typeof(Node<>).MakeGenericType(option)) as Node;
        node.style.top = style.top;
        node.style.left = style.left;
        parent.Add(node);
        parent.Remove(this);
    }
}

public abstract class Node : VisualElement
{
    bool drag = false;
    private Vector2 startPos;
    public Node()
    {
        style.borderTopWidth = style.borderBottomWidth = style.borderLeftWidth = style.borderRightWidth = 1f;
        style.borderTopLeftRadius = style.borderBottomLeftRadius = style.borderBottomRightRadius = style.borderTopRightRadius = 5f;
        style.borderBottomColor = style.borderLeftColor = style.borderRightColor = style.borderTopColor = Color.grey * .1f;
        style.paddingBottom = style.paddingLeft = style.paddingTop = style.paddingRight = 5f;
        style.width = 300f;
        style.height = 200f;
        style.position = Position.Absolute;
        style.backgroundColor = Color.grey * .9f;
        var label = new Label(this.name);
        Add(label);
        Add(new Label("name:"));
        var nameInput = new TextField();
        Add(nameInput);

        var button = new Button(() => parent.Remove(this));
        button.Add(new Label("delete"));
        Add(button);

        this.AddManipulator(new NodeDragger());
    }
}

public class Node<T> : Node where T : new()
{
    protected T data;
    public T Data => data;

    public Node() : base()
    {
        data = new T();

        var fields = data.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
        for(int i = 0; i < fields.Length; i++)
        {
            if(fields[i].FieldType.IsValueType)
            {
                Add(new Label($"{fields[i].Name}:"));
                var input = new TextField();
                input.value = fields[i].GetValue(data) as string;
                Add(input);
                input.RegisterValueChangedCallback(value => fields[i].SetValue(data, value));
            }
        }
    }
}

public class NodeDragger : MouseManipulator
{
    #region Init
    private Vector2 m_Start;
    protected bool m_Active;

    public NodeDragger()
    {
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
        m_Active = false;
    }
    #endregion

    #region Registrations
    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
    }
    #endregion

    #region OnMouseDown
    protected void OnMouseDown(MouseDownEvent e)
    {
        if (m_Active)
        {
            e.StopImmediatePropagation();
            return;
        }

        if (CanStartManipulation(e))
        {
            m_Start = e.localMousePosition;

            m_Active = true;
            target.CaptureMouse();
            e.StopPropagation();
        }
    }
    #endregion

    #region OnMouseMove
    protected void OnMouseMove(MouseMoveEvent e)
    {
        if (!m_Active || !target.HasMouseCapture())
            return;

        Vector2 diff = e.localMousePosition - m_Start;

        target.style.top = target.layout.y + diff.y;
        target.style.left = target.layout.x + diff.x;

        e.StopPropagation();
    }
    #endregion

    #region OnMouseUp
    protected void OnMouseUp(MouseUpEvent e)
    {
        if (!m_Active || !target.HasMouseCapture() || !CanStopManipulation(e))
            return;

        m_Active = false;
        target.ReleaseMouse();
        e.StopPropagation();
    }
    #endregion
}