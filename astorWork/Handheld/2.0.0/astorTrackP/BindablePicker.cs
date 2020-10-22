using System;
using Xamarin.Forms;
using System.Collections;
using System.Reflection;
using System.Collections.Specialized;
using System.Linq;

namespace astorTrackP
{
	public class BindablePicker : Picker
	{
    	public BindablePicker()
        {
            this.SelectedIndexChanged += OnSelectedIndexChanged;
		} 
       
//        public static BindableProperty ItemsSourceProperty =
//            BindableProperty.Create<BindablePicker, IEnumerable>(o => o.ItemsSource, default(IEnumerable), propertyChanged: OnItemsSourceChanged);
//
//        public static BindableProperty SelectedItemProperty =
//			BindableProperty.Create<BindablePicker, object>(o => o.SelectedItem, default(object),BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

		public static readonly BindableProperty SelectedItemProperty = 
			BindableProperty.Create("SelectedItem", typeof(object), typeof(BindablePicker), null, BindingMode.TwoWay, null, new BindableProperty.BindingPropertyChangedDelegate(BindablePicker.OnSelectedItemChanged), null, null, null);

		public static readonly BindableProperty ItemsSourceProperty = 
			BindableProperty.Create("ItemsSource", typeof(IEnumerable), typeof(BindablePicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(BindablePicker.OnItemsSourceChanged), null, null, null);

		public static readonly BindableProperty DisplayPropertyProperty = 
			BindableProperty.Create("DisplayProperty", typeof(string), typeof(BindablePicker), null, BindingMode.OneWay, null, new BindableProperty.BindingPropertyChangedDelegate(BindablePicker.OnDisplayPropertyChanged), null, null, null);

        public IList ItemsSource
        {
			get { return (IList)GetValue(BindablePicker.ItemsSourceProperty); }
			set { SetValue(BindablePicker.ItemsSourceProperty, value); }
        }

//        public object SelectedItem
//        {
//            get { return (object)GetValue(SelectedItemProperty); }
//            set { SetValue(SelectedItemProperty, value); }
//        }

		public object SelectedItem
		{
			get
			{
				return base.GetValue(BindablePicker.SelectedItemProperty);
			}
			set
			{
				base.SetValue(BindablePicker.SelectedItemProperty, value);
			}
		}

        private static void OnDisplayPropertyChanged(BindableObject bindable, object oldValue, object newValue)
		{
			BindablePicker picker = (BindablePicker)bindable;
			picker.DisplayProperty = (string)newValue;
		}

		private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			BindablePicker bindablePicker = (BindablePicker)bindable;
			bindablePicker.ItemsSource = (IList)newValue;
			loadItemsAndSetSelected (bindable);

//			var notifyCollection = newValue as INotifyCollectionChanged;
//			if (notifyCollection != null)
//			{
//				notifyCollection.CollectionChanged += (sender, args) =>
//				{
//					if (args.NewItems != null)
//					{
//						foreach (var newItem in args.NewItems)
//						{
//							bindablePicker.Items.Add((newItem ?? "").ToString());
//						}
//					}
//					if (args.OldItems != null)
//					{
//						foreach (var oldItem in args.OldItems)
//						{
//							bindablePicker.Items.Remove((oldItem ?? "").ToString());
//						}
//					}
//				};
//			}
		}

		static void loadItemsAndSetSelected (BindableObject bindable)
		{
			BindablePicker bindablePicker = (BindablePicker)bindable;
			if (bindablePicker.ItemsSource as IEnumerable != null) {
				PropertyInfo propertyInfo = null;
				int count = 0;
				foreach (object obj in (IEnumerable)bindablePicker.ItemsSource) {
					string value = string.Empty;
					if (bindablePicker.DisplayProperty != null) {
						if (propertyInfo == null) {
							propertyInfo = obj.GetType ().GetRuntimeProperty (bindablePicker.DisplayProperty);
							if (propertyInfo == null)
								throw new Exception (String.Concat (bindablePicker.DisplayProperty, " is not a property of ", obj.GetType ().FullName));
						}
						value = propertyInfo.GetValue (obj).ToString();
					}
					else {
						value = obj.ToString();
					}

					bindablePicker.Items.Add (value);
//					if (bindablePicker.SelectedItem != null) {
//						if (bindablePicker.SelectedItem == obj) {
//							bindablePicker.SelectedIndex = count;
//						}
//					}

					count++;
				}
			}
		}

		public int GetSelectedIndex(string value)
		{
		
			int selectedValue = 0;
			if (ItemsSource as IEnumerable != null) {
				PropertyInfo propertyInfo = null;

				foreach (object obj in (IEnumerable)ItemsSource) {
					string propValue = string.Empty;
					if (DisplayProperty != null) {
						if (propertyInfo == null) {
							propertyInfo = obj.GetType ().GetRuntimeProperty (DisplayProperty);
							if (propertyInfo == null)
								throw new Exception (String.Concat (DisplayProperty, " is not a property of ", obj.GetType ().FullName));
						}
						propValue = propertyInfo.GetValue (obj).ToString();
					}
					else {
						propValue = obj.ToString();
					}

					if (propValue == value)
						break;
					
					selectedValue++;
				}
			}

			return selectedValue;
		}

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
			var picker = sender as BindablePicker;
            if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
            {
                SelectedItem = null;
            }
            else
            {
				SelectedItem = Items[SelectedIndex];
//				SelectedItem = picker.ItemsSource[SelectedIndex];
            }
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            var picker = bindable as BindablePicker;
            if (newvalue != null)
            {
                picker.SelectedIndex = picker.Items.IndexOf(newvalue.ToString());
            }

        }

//		private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
//		{
//			BindablePicker bindablePicker = (BindablePicker)bindable;
//			bindablePicker.SelectedItem = newValue;
//			if (bindablePicker.ItemsSource != null && bindablePicker.SelectedItem!=null) {
//				int count = 0;
//				foreach (object obj in bindablePicker.ItemsSource) {
//					if (obj == bindablePicker.SelectedItem) {
//						bindablePicker.SelectedIndex = count;
//						break;
//					}
//					count++;
//				}
//			}
//		}

//		private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
//		{
//			var picker = bindable as BindablePicker;
//			if (newvalue != null)
//			{
//				picker.SelectedIndex = picker.Items.IndexOf(newvalue.ToString());
//			}
//		}

		public string DisplayProperty
		{
			get { return (string)base.GetValue(BindablePicker.DisplayPropertyProperty); }
			set { base.SetValue(BindablePicker.DisplayPropertyProperty, value); }
		}
    }
}


