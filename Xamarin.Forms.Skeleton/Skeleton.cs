using System;
using System.Collections.Generic;
using System.Linq;
#if NET6_0_OR_GREATER
using Maui.Skeleton.Animations;
using Maui.Skeleton.Extensions;
#else
using Xamarin.Forms.Skeleton.Animations;
using Xamarin.Forms.Skeleton.Extensions;
#endif


#if NET6_0_OR_GREATER
namespace Maui.Skeleton
#else
namespace Xamarin.Forms.Skeleton
#endif
{
    public static class Skeleton
    {
        #region Public Properties

        public static readonly BindableProperty MockItemTemplateProperty = BindableProperty.CreateAttached("MockItemTemplate", typeof(DataTemplate), typeof(View), defaultValue: null);

        public static DataTemplate GetMockItemTemplate(BindableObject b) => (DataTemplate)b.GetValue(MockItemTemplateProperty);

        public static void SetMockItemTemplate(BindableObject b, DataTemplate value) => b.SetValue(MockItemTemplateProperty, value);

        public static readonly BindableProperty MockItemNumberProperty = BindableProperty.CreateAttached("MockItemNumber", typeof(int), typeof(View), defaultValue: -1);

        public static void SetMockItemNumber(BindableObject b, int value) => b.SetValue(MockItemNumberProperty, value);

        public static int GetMockItemNumber(BindableObject b) => (int)b.GetValue(MockItemNumberProperty);

        public static readonly BindableProperty IsParentProperty = BindableProperty.CreateAttached("IsParent", typeof(bool), typeof(View), false);

        public static void SetIsParent(BindableObject b, bool value) => b.SetValue(IsParentProperty, value);

        public static bool GetIsParent(BindableObject b) => (bool)b.GetValue(IsParentProperty);

        public static readonly BindableProperty IsBusyProperty = BindableProperty.CreateAttached("IsBusy", typeof(bool), typeof(View), default(bool), propertyChanged: (b, oldValue, newValue) => OnIsBusyChanged(b, (bool)newValue));

        public static void SetIsBusy(BindableObject b, bool value) => b.SetValue(IsBusyProperty, value);

        public static bool GetIsBusy(BindableObject b) => (bool)b.GetValue(IsBusyProperty);

        public static readonly BindableProperty HideProperty = BindableProperty.CreateAttached("Hide", typeof(bool), typeof(View), default(bool));

        public static void SetHide(BindableObject b, bool value) => b.SetValue(HideProperty, value);

        public static bool GetHide(BindableObject b) => (bool)b.GetValue(HideProperty);

        public static readonly BindableProperty BackgroundColorProperty = BindableProperty.CreateAttached("BackgroundColor", typeof(Color), typeof(View), default(Color));

        public static void SetBackgroundColor(BindableObject b, Color value) => b.SetValue(BackgroundColorProperty, value);

        public static Color GetBackgroundColor(BindableObject b) => (Color)b.GetValue(BackgroundColorProperty);

        public static readonly BindableProperty AnimationProperty = BindableProperty.CreateAttached("Animation", typeof(BaseAnimation), typeof(View), null);

        public static void SetAnimation(BindableObject b, BaseAnimation value) => b.SetValue(AnimationProperty, value);

        public static BaseAnimation GetAnimation(BindableObject b) => (BaseAnimation)b.GetValue(AnimationProperty);

        #endregion Public Properties

        #region Internal Properties

        internal static readonly BindableProperty AnimatingProperty = BindableProperty.CreateAttached("Animating", typeof(bool), typeof(View), default(bool));

        internal static void SetAnimating(BindableObject b, bool value) => b.SetValue(AnimatingProperty, value);

        internal static bool GetAnimating(BindableObject b) => (bool)b.GetValue(AnimatingProperty);

        internal static readonly BindableProperty CancelAnimationProperty = BindableProperty.CreateAttached("CancelAnimation", typeof(bool), typeof(View), default(bool));

        internal static void SetCancelAnimation(BindableObject b, bool value) => b.SetValue(CancelAnimationProperty, value);

        internal static bool GetCancelAnimation(BindableObject b) => (bool)b.GetValue(CancelAnimationProperty);

        internal static readonly BindableProperty OriginalBackgroundColorProperty = BindableProperty.CreateAttached("OriginalBackgroundColor", typeof(Color), typeof(View), default(Color));

        internal static void SetOriginalBackgroundColor(BindableObject b, Color value) => b.SetValue(OriginalBackgroundColorProperty, value);

        internal static Color GetOriginalBackgroundColor(BindableObject b) => (Color)b.GetValue(OriginalBackgroundColorProperty);

        internal static readonly BindableProperty UseDynamicTextColorProperty = BindableProperty.CreateAttached("UseDynamicTextColor", typeof(bool), typeof(View), default(bool));

        internal static void SetUseDynamicTextColor(BindableObject b, bool value) => b.SetValue(UseDynamicTextColorProperty, value);

        internal static bool GetUseDynamicTextColor(BindableObject b) => (bool)b.GetValue(UseDynamicTextColorProperty);

        internal static readonly BindableProperty UseDynamicBackgroundColorProperty = BindableProperty.CreateAttached("UseDynamicBackground", typeof(bool), typeof(View), default(bool));

        internal static bool GetUseDynamicBackgroundColor(BindableObject b) => (bool)b.GetValue(UseDynamicBackgroundColorProperty);

        internal static void SetUseDynamicBackgroundColor(BindableObject b, bool value) => b.SetValue(UseDynamicBackgroundColorProperty, value);

        internal static readonly BindableProperty OriginalTextColorProperty = BindableProperty.CreateAttached("OriginalTextColor", typeof(Color), typeof(View), default(Color));

        internal static void SetOriginalTextColor(BindableObject b, Color value) => b.SetValue(OriginalTextColorProperty, value);

        internal static Color GetOriginalTextColor(BindableObject b) => (Color)b.GetValue(OriginalTextColorProperty);

        #endregion Internal Properties

        #region Private Fields

        static Dictionary<Guid, DataTemplate> _internalListDataTemplates = new Dictionary<Guid, DataTemplate>();
        static Dictionary<Guid, Binding> _listViewBindings = new Dictionary<Guid, Binding>();

        #endregion Private Fields

        #region Operations

        private static void OnIsBusyChanged(BindableObject bindable, bool newValue)
        {
            if (bindable.GetType().IsSubclassOf(typeof(View)))
            {
                HandleIsBusyChanged(bindable, newValue);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        static void HandleIsBusyChanged(BindableObject bindable, bool isBusyNewValue)
        {
            if (!(bindable is View))
                return;

            var view = (View)bindable;
            if (isBusyNewValue)
            {
                if (GetHide(bindable))
                {
                    ((View)bindable).IsVisible = false;
                }
                else
                {
                    if (view is ListView listView)
                        CreateMockupList(bindable, listView);
                    else if (view is CollectionView collectionView)
                        CreateMockupList(bindable, collectionView);

                    if (view is Layout layout && !GetIsParent(bindable))
                    {
                        SetLayoutChilds(layout);
                    }
                    else if (view is Label || view is Button)
                    {
                        SetTextColor(view);
                    }

                    SetBackgroundColor(view);

                    RunAnimation(view);
                }
            }
            else
            {
                if (GetHide(bindable))
                {
                    ((View)bindable).IsVisible = true;
                }
                else
                {

                    if (view is ListView listView)
                        RemoveMockupList(bindable, listView);
                    else if (view is CollectionView collectionView)
                        RemoveMockupList(bindable, collectionView);

                    CancelAnimation(view);

                    RestoreBackgroundColor(view);

                    if (view is Layout layout && !GetIsParent(bindable))
                    {
                        RestoreLayoutChilds(layout);
                    }
                    else if (view is Label || view is Button)
                    {
                        RestoreTextColor(view);
                    }
                }
            }
        }

        private static void SetLayoutChilds(Layout layout)
        {
            if (layout.Children != null && layout.Children.Count > 0)
            {
                layout.Children.ToList().ForEach(x => ((View)x).SetValue(VisualElement.OpacityProperty, 0));
            }
        }

        private static void CreateMockupList(BindableObject bindable, ListView listView)
        {
            DataTemplate template = GetMockItemTemplate(bindable);
            int mockNumber = GetMockItemNumber(bindable);
            Guid viewId = listView.Id;

            if (template != null)
            {
                if (mockNumber <= 0)
                    throw new InvalidOperationException("MockNumber must be greater than 0 to use MockTemplate");

                //Save Template
                _internalListDataTemplates[viewId] = listView.ItemTemplate;
                listView.ItemTemplate = template;
            }

            if (mockNumber > 0) {
                // Get Binding
                _listViewBindings[viewId] = listView.GetBinding(ListView.ItemsSourceProperty);

                if (_listViewBindings[viewId] is null)
                    throw new InvalidOperationException("Must create Binding to ListView's ItemsSource");

                listView.RemoveBinding(ListView.ItemsSourceProperty);

                List<object> list = new List<object>(mockNumber);

                for (int i = 0; i < mockNumber; i++)
                {
                    list.Add(new object());
                }

                listView.ItemsSource = list;
            }
        }
        private static void CreateMockupList(BindableObject bindable, CollectionView collectionView)
        {
            DataTemplate template = GetMockItemTemplate(bindable);
            int mockNumber = GetMockItemNumber(bindable);
            Guid viewId = collectionView.Id;

            if (template != null)
            {
                if (mockNumber <= 0)
                    throw new InvalidOperationException("MockNumber must be greater than 0 to use MockTemplate");

                //Save Template
                _internalListDataTemplates[viewId] = collectionView.ItemTemplate;
                collectionView.ItemTemplate = template;
            }

            if (mockNumber > 0)
            {
                //Get Binding
                _listViewBindings[viewId] = collectionView.GetBinding(ListView.ItemsSourceProperty);

                if (_listViewBindings[viewId] is null)
                    throw new InvalidOperationException("Must create Binding to ListView's ItemsSource");

                collectionView.RemoveBinding(ListView.ItemsSourceProperty);

                List<object> list = new List<object>(mockNumber);

                for (int i = 0; i < mockNumber; i++)
                {
                    list.Add(new object());
                }

                collectionView.ItemsSource = list;
            }
        }

        private static void RemoveMockupList(BindableObject bindable, ListView listView)
        {
            Guid viewId = listView.Id;
            if (_listViewBindings.ContainsKey(viewId))
            {
                listView.ItemsSource = null;

                if (_internalListDataTemplates.ContainsKey(viewId))
                {
                    listView.ItemTemplate = _internalListDataTemplates[viewId];
                }

                listView.SetBinding(ListView.ItemsSourceProperty, _listViewBindings[viewId]);
            }
        }
        private static void RemoveMockupList(BindableObject bindable, CollectionView collectionView)
        {
            Guid viewId = collectionView.Id;
            if (_listViewBindings.ContainsKey(viewId))
            {
                collectionView.ItemsSource = null;

                if (_internalListDataTemplates.ContainsKey(viewId))
                {
                    collectionView.ItemTemplate = _internalListDataTemplates[viewId];
                }

                collectionView.SetBinding(ListView.ItemsSourceProperty, _listViewBindings[viewId]);
            }
        }

        private static void RestoreLayoutChilds(Layout layout)
        {
            if (layout.Children != null && layout.Children.Count > 0)
            {
                layout.Children.ToList().ForEach(x => ((View)x).SetValue(VisualElement.OpacityProperty, 1));
            }
        }

        private static void SetBackgroundColor(View view)
        {
            var hasDynamic = GetUseDynamicBackgroundColor(view)
                || view.HasDynamicColorOnProperty(View.BackgroundColorProperty);

            var backgroundColor = GetBackgroundColor(view);
            if (backgroundColor != default(Color))
            {
                SetOriginalBackgroundColor(view, view.BackgroundColor);
                view.BackgroundColor = backgroundColor;
            }

            SetUseDynamicBackgroundColor(view, hasDynamic);
        }

        private static void RestoreBackgroundColor(View view)
        {
            var useDynamic = GetUseDynamicBackgroundColor(view);
            var backgroundColor = GetBackgroundColor(view);
            if (useDynamic)
            {
                view.ClearValue(View.BackgroundColorProperty);
            }
            else if (backgroundColor != default(Color))
            {
                view.BackgroundColor = GetOriginalBackgroundColor(view);
            }
        }

        private static void SetTextColor(View view)
        {
            var hasDynamic = GetUseDynamicTextColor(view);
            if (view is Label label)
            {
                hasDynamic = hasDynamic || label.HasDynamicColorOnProperty(Label.TextColorProperty);
                SetOriginalTextColor(label, label.TextColor);
#if NET6_0_OR_GREATER
                label.TextColor = Colors.Transparent;
#else
                label.TextColor = Color.Transparent;
#endif
            }
            else if (view is Button button)
            {
                hasDynamic = hasDynamic || button.HasDynamicColorOnProperty(Button.TextColorProperty);
                SetOriginalTextColor(button, button.TextColor);
#if NET6_0_OR_GREATER
                button.TextColor = Colors.Transparent;
#else
                button.TextColor = Color.Transparent;
#endif
            }

            SetUseDynamicTextColor(view, hasDynamic);
        }

        private static void RestoreTextColor(View view)
        {
            var useDynamic = GetUseDynamicTextColor(view);
            if (view is Label label)
            {
                if (useDynamic)
                {
                    var key = label.GetPropertyDynamicResourceKey(Label.TextColorProperty);
                    label.SetDynamicResource(Label.TextColorProperty, key);
                }
                else
                {
                    label.TextColor = GetOriginalTextColor(view);
                }
            }
            else if (view is Button button)
            {
                if (useDynamic)
                {
                    var key = button.GetPropertyDynamicResourceKey(Button.TextColorProperty);
                    button.SetDynamicResource(Button.TextColorProperty, key);
                }
                else
                {
                    button.TextColor = GetOriginalTextColor(view);
                }
            }
        }

        private static void RunAnimation(View view)
        {
            var animation = GetAnimation(view);

            if (animation == null || GetAnimating(view))
                return;

            SetCancelAnimation(view, false);

            if (animation != null)
            {
                animation.Start(view);
            }
        }

        private static void CancelAnimation(View view)
        {
            var animation = GetAnimation(view);

            if (animation == null)
                return;

            SetCancelAnimation(view, true);

            animation.Stop(view);
        }

        #endregion Operations
    }
}
