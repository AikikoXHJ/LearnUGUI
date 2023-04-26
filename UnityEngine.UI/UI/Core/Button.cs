using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    /// <summary>
    /// Button that's meant to work with mouse or touch-based devices.
    /// 在鼠标或触控设备使用的按钮，被点击的时候发送事件
    /// Button 继承了 Selectable 类，这是 UI 系统中的一个基类
    /// 主要作用是提供了一些基本的交互功能，例如选中、高亮、禁用等
    /// 还提供了一些虚拟方法，可以通关重写这些方法来实现自定义的交互外观或行为
    /// 实现了以下接口：
    /// IPointerClickHandler：用于注册按钮点击事件，可以用来判断点击类型（左键、右键等）
    /// ISubmitHandler：用于注册提交事件，例如在输入框中按下回车的时候
    /// </summary>
    [AddComponentMenu("UI/Button", 30)]
    public class Button : Selectable, IPointerClickHandler, ISubmitHandler
    {
        [Serializable]
        /// <summary>
        /// Function definition for a button click event.
        /// 定义了一个按钮的点击事件
        /// </summary>
        public class ButtonClickedEvent : UnityEvent {}

        // Event delegates triggered on click.
        // 实例化一个 ButtonClickedEvent 的事件
        [FormerlySerializedAs("onClick")]
        [SerializeField]
        private ButtonClickedEvent m_OnClick = new ButtonClickedEvent();

        // 保证 Button 类只能被继承而不能被实例化
        protected Button()
        {}

        /// <summary>
        /// UnityEvent that is triggered when the button is pressed.
        /// 当按钮被按下的时候触发 UnityEvent
        /// Note: Triggered on MouseUp after MouseDown on the same object.
        /// 注意：是在同一对象的 MouseDown 之后当 MouseUp 的时候触发
        /// 下面是一个简单的 Button 点击事件的实现代码
        /// </summary>
        ///<example>
        ///<code>
        /// using UnityEngine;
        /// using UnityEngine.UI;
        /// using System.Collections;
        ///
        /// public class ClickExample : MonoBehaviour
        /// {
        ///     public Button yourButton;
        ///
        ///     void Start()
        ///     {
        ///         Button btn = yourButton.GetComponent<Button>();
        ///         btn.onClick.AddListener(TaskOnClick);
        ///     }
        ///
        ///     void TaskOnClick()
        ///     {
        ///         Debug.Log("You have clicked the button!");
        ///     }
        /// }
        ///</code>
        ///</example>

        // 常用的 onClick.AddListener() 就是监听这个事件
        public ButtonClickedEvent onClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
        }

        /// <summary>
        /// 如果按钮处于活跃且可交互状态（ Interactable 设置为 true），则触发该事件
        /// </summary>
        private void Press()
        {
            if (!IsActive() || !IsInteractable())
                return;

            UISystemProfilerApi.AddMarker("Button.onClick", this);
            m_OnClick.Invoke();
        }

        /// <summary>
        /// Call all registered IPointerClickHandlers.
        /// 调用所有已注册的 IPointerClickHandlers
        /// Register button presses using the IPointerClickHandler. You can also use it to tell what type of click happened (left, right etc.).
        /// IPointerClickHandlers 接口用于注册按钮点击事件，可以通过它来判断点击的类型（左、右键等）
        /// Make sure your Scene has an EventSystem.
        /// 确保你的场景中有一个 EventSystem
        /// </summary>
        /// <param name="eventData">Pointer Data associated with the event. Typically by the event system.</param>
        /// <example>
        /// <code>
        /// //Attatch this script to a Button GameObject
        /// 示例代码，将脚本添加到 Button GameObject
        /// using UnityEngine;
        /// using UnityEngine.EventSystems;
        ///
        /// public class Example : MonoBehaviour, IPointerClickHandler
        /// {
        ///     //Detect if a click occurs
        ///     // 检测是否发生了点击
        ///     public void OnPointerClick(PointerEventData pointerEventData)
        ///     {
        ///             //Use this to tell when the user right-clicks on the Button
        ///             // 判断是不是右键点击
        ///         if (pointerEventData.button == PointerEventData.InputButton.Right)
        ///         {
        ///             //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
        ///             Debug.Log(name + " Game Object Right Clicked!");
        ///         }
        ///
        ///         //Use this to tell when the user left-clicks on the Button
        ///         // 判断是不是左键点击
        ///         if (pointerEventData.button == PointerEventData.InputButton.Left)
        ///         {
        ///             Debug.Log(name + " Game Object Left Clicked!");
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>

        // 重写 IPointerClickHandler 接口中的方法 OnPointerClick
        // 鼠标左键点击时调用 Press() 方法
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            Press();
        }

        /// <summary>
        /// Call all registered ISubmitHandler.
        /// 调用所有已注册的 ISubmitHandler
        /// </summary>
        /// <param name="eventData">Associated data with the event. Typically by the event system.</param>
        /// <remarks>
        /// This detects when a Button has been selected via a "submit" key you specify (default is the return key).
        /// ISubmitHandler 接口用于当用户按下指定的“提交”键（默认是回车键）时触发
        ///
        ///-------------- 下面是如何修改 submit 默认键
        /// To change the submit key, either:
        ///
        /// 1. Go to Edit->Project Settings->Input.
        ///
        /// 2. Next, expand the Axes section and go to the Submit section if it exists.
        ///
        /// 3. If Submit doesn’t exist, add 1 number to the Size field. This creates a new section at the bottom. Expand the new section and change the Name field to “Submit”.
        ///
        /// 4. Change the Positive Button field to the key you want (e.g. space).
        ///
        ///
        /// Or:
        ///
        /// 1. Go to your EventSystem in your Project
        ///
        /// 2. Go to the Inspector window and change the Submit Button field to one of the sections in the Input Manager (e.g. "Submit"), or create your own by naming it what you like, then following the next few steps.
        ///
        /// 3. Go to Edit->Project Settings->Input to get to the Input Manager.
        ///
        /// 4. Expand the Axes section in the Inspector window. Add 1 to the number in the Size field. This creates a new section at the bottom.
        ///
        /// 5. Expand the new section and name it the same as the name you inserted in the Submit Button field in the EventSystem. Set the Positive Button field to the key you want (e.g. space)
        /// </remarks>

        // 重写 ISubmitHandler 接口中的方法，当按下“提交”按键的时候
        // 调用 OnSubmit 方法，进而调用 Press() 方法
        // 执行 DoStateTransition 方法，设置 Button 的状态为UI对象被按下，并且是需要过度（这里主要是为UI显示做设置）
        // 如果在 Press 方法执行期间，Button 组件被禁用或者设置为不可交互状态
        // 那么 OnSubmit 方法会直接返回，不会执行状态设置和协程操作
        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable())
                return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        /// <summary>
        /// OnSubmit 方法中执行的协程操作
        /// 该方法的意义是等到 Button 按钮的颜色渐变完成
        /// 然后执行 DoStateTransition 方法， 将 Button 的状态设置为当前选择状态
        /// 在这个过程中，如果 Button 组件被禁用或者设置为不可交互的状态，协程会直接返回
        /// </summary>
        /// <returns></returns>
        private IEnumerator OnFinishSubmit()
        {
            var fadeTime = colors.fadeDuration;
            var elapsedTime = 0f;

            while (elapsedTime < fadeTime)
            {
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            DoStateTransition(currentSelectionState, false);
        }
    }
}
