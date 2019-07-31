using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RocketWorks.Entities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
 
public class ContextEditor : NodeCanvasEditor<GameContextData>
{
    [MenuItem("RocketWorks/Context Editor")]
    public static void OpenWindow()
    {
        ContextEditor window = (ContextEditor)EditorWindow.GetWindow(typeof(ContextEditor));
        window.Show();
    }
}

public class EntityEditor : NodeCanvasEditor<Entity>
{ 
    [MenuItem("RocketWorks/Entity Editor")]
    public static void OpenWindow()
    {
        EntityEditor window = (EntityEditor)EditorWindow.GetWindow(typeof(EntityEditor));
        window.Show();
    }
}

public abstract class NodeCanvasEditor<T> : EditorWindow
{
    public void OnEnable()
    {
        var root = this.rootVisualElement;

        var nodeCanvas = new NodeCanvas<T>();
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
    public List<ComponentData> componentTypes;
}

public class ContextData
{
    public string contextName;
    public List<ComponentData> components;
}

public class ComponentData
{
    public string componentName;
}

public class FieldData
{
    
}

public class NodeCanvas<T> : VisualElement
{
    private VisualElement menu;

    public NodeCanvas() : base()
    {
        menu = new RightClickMenu<T>();
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

public class RightClickMenu<T> : VisualElement
{
    private List<Type> options = new List<Type>();

    public RightClickMenu()
    {
        
        List<Type> typesToCheck = new List<Type>{typeof(T)};
        int iter = 0;
        while(typesToCheck.Count > 0 && iter < 100)
        {
            iter++;
            var checkingType = typesToCheck[0];
            typesToCheck.RemoveAt(0);
            
            if(typesToCheck.Contains(checkingType) || options.Contains(checkingType))
                continue;

            var fields = checkingType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            for(int i = 0; i < fields.Length; i++)
            {
                var fieldType = fields[i].FieldType;
                typesToCheck.Add(fieldType);
            }

            if(checkingType.IsGenericType)
            {
                var generics = checkingType.GetGenericArguments();
                typesToCheck.AddRange(generics);
            }

            if(checkingType.IsArray)
            {
                typesToCheck.Add(checkingType.GetElementType());
                continue;
            }

            if(checkingType.IsInterface)
            {
                typesToCheck.AddRange(
                    Assembly.GetAssembly(checkingType).GetTypes().Where(x => x.GetInterfaces().Length >= 1 && x.GetInterfaces().Contains(checkingType) && x.IsClass && !x.IsAbstract)
                    );
                continue;
            }

            if(checkingType.IsValueType)
                continue;
            options.Add(checkingType);
        }
        
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

    protected Label mainLabel;

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
        mainLabel = new Label(this.name);
        Add(mainLabel);

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
        mainLabel.text = typeof(T).Name;
        data = new T();

        var fields = data.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
        for(int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            if(field.FieldType.IsValueType || field.FieldType == typeof(string))
            {
                Add(new Label($"{field.Name}:"));
                var input = new TextField();
                input.value = field.GetValue(data) as string;
                Add(input);
                input.RegisterValueChangedCallback(value => {
                    field.SetValue(data, value.newValue);
                    Debug.Log(JsonUtility.ToJson(data));
                    });
                
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