using System;
using Myra.Utility.Types;
#if MATH_IFACES
using System.Numerics;
#endif

namespace Myra.Graphics2D.UI.Properties
{
	[PropertyEditor(typeof(NumericPropertyEditor<>), 
		typeof(byte), typeof(sbyte), typeof(byte?), typeof(sbyte?),
		typeof(short), typeof(ushort), typeof(short?), typeof(ushort?), 
		typeof(int), typeof(uint), typeof(int?), typeof(uint?), 
		typeof(long), typeof(ulong), typeof(long?), typeof(ulong?),
		typeof(float), typeof(float?), typeof(double), typeof(double?), 
		typeof(decimal), typeof(decimal?))]
	public sealed class NumericPropertyEditor<TNum> : StructPropertyEditor<TNum>, INumberRecordReference<TNum> 
#if MATH_IFACES
		where TNum : struct, INumber<TNum>, IMinMaxValue<TNum>
#else
		where TNum : struct
#endif
	{
		private Type _underlyingType;
		private bool _nullable;
		private bool _doByteDodge;
		public bool IsNullable => _nullable;
	    public NumericPropertyEditor(IInspector owner, Record methodInfo) : base(owner, methodInfo)
	    {
	    }
	    protected override void Initialize()
	    {
		    _underlyingType = Type;
		    _nullable = TypeHelper.GetNullableTypeOrPassThrough(ref _underlyingType);
		    _doByteDodge = _underlyingType == typeof(byte) | _underlyingType == typeof(sbyte);
		    
		    Type selfType = typeof(TNum);
		    Type copy = selfType;
		    TypeHelper.GetNullableTypeOrPassThrough(ref copy);
		    if (_underlyingType != copy)
			    throw new TypeLoadException($"Type mismatch: Generic type arg '{selfType}' is unequatable to assigned Record type '{Type}'");
		    if (selfType.IsGenericType)
			    throw new TypeLoadException($"Generic types are not a valid generic argument for this type: {selfType}");
		    if (Type.IsGenericType && Type.GetGenericTypeDefinition() != typeof(Nullable<>))
			    throw new TypeLoadException($"Record provided is a generic but not Nullable: {Type}");
	    }
	    
	    protected override bool CreatorPicker(out WidgetCreatorDelegate creatorDelegate)
	    {
		    creatorDelegate = CreateNumericEditor;
		    return true;
	    }
	    private bool CreateNumericEditor(out Widget widget)
        {
	        if (_owner.SelectedField == null)
	        {
		        widget = null;
		        return false;
	        }
	        
	        object obj = _record.GetValue(_owner.SelectedField);
	        TNum? convert;
	        if (_nullable)
	        {
		        convert = (TNum?)obj;
	        }
	        else
	        {
		        if (obj == null)
			        convert = MathHelper<TNum>.Zero;
		        else
			        convert = (TNum)obj;
	        }

	        if (_doByteDodge)
	        {
		        widget = CreateByteDodge(convert);
	        }
	        else
	        {
		        widget = CreateNativeType(convert);
	        }

	        Widget = widget;
	        return true;
        }

        private Widget CreateNativeType(TNum? val)
        {
	        var spinButton = new SpinButton<TNum>()
	        {
		        Nullable = _nullable,
		        Value = val,
	        };

	        if (_record.HasSetter)
	        {
		        spinButton.ValueChanged += (sender, args) =>
		        {
			        try
			        {
				        if (IsNullable)
					        SetValue(_owner.SelectedField, args.NewValue);
				        else
					        SetValue(_owner.SelectedField, args.NewValue.GetValueOrDefault());

				        _owner.FireChanged(_record.Name);
			        }
			        catch (Exception ex)
			        {
				        spinButton.Value = args.OldValue;
				        var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
				        dialog.ShowModal(_owner.Desktop);
			        }
		        };
	        }
	        else
	        {
		        spinButton.Enabled = false;
	        }
	        return spinButton;
        }

        private Widget CreateByteDodge(TNum? val)
        {
	        // val is byte or sbyte which doesn't have math ops
	        var spinButton = new SpinButton<short>()
	        {
		        Nullable = _nullable,
		        Value = NullableConvert(val),
		        Minimum = 0,
		        Maximum = 255,
	        };

	        if (_record.HasSetter)
	        {
		        spinButton.ValueChanged += (sender, args) =>
		        {
			        try
			        {
				        short? newShort = args.NewValue;
				        if (!newShort.HasValue)
				        {
					        if(IsNullable)
					        {
						        SetValue(_owner.SelectedField, null);
						        _owner.FireChanged(_record.Name);
						        return;
					        }
					        else
					        {
						        SetValue(_owner.SelectedField, MathHelper<TNum>.Zero);
						        _owner.FireChanged(_record.Name);
						        return;
					        }
				        }
				        
				        newShort = MathHelper<short>.Clamp(newShort.Value, 0, 255);
				        SetValue(_owner.SelectedField, MathHelper<short, TNum>.Convert(newShort.Value));
				        _owner.FireChanged(_record.Name);
			        }
			        catch (Exception ex)
			        {
				        spinButton.Value = args.OldValue;
				        var dialog = Dialog.CreateMessageBox("Error", ex.ToString());
				        dialog.ShowModal(_owner.Desktop);
			        }
		        };
	        }
	        else
	        {
		        spinButton.Enabled = false;
	        }
	        return spinButton;
        }
        
        public override void SetWidgetValue(TNum? value)
        {
	        if (Widget is SpinButton<TNum> native)
		        native.Value = value;
	        else if(Widget is SpinButton<short> dodge)
	        {
		        short? val;
		        if (!IsNullable && !value.HasValue)
		        {
			        val = 0;
		        }
		        else
		        {
			        val = NullableConvert(value);
		        }
		        dodge.Value = val;
	        }
        }

        private static short? NullableConvert(TNum? value)
        {
	        short? convert = null;
	        if (value.HasValue)
		        convert = MathHelper<TNum, short>.Convert(value.Value);
	        return convert;
        }
	}
}