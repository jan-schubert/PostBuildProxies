using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace PostBuildProxies.Core.Generation.PropertyGenerators
{
    public class NotifyPropertyChangedGenerator
        : INotifyPropertyChangedGenerator
    {
        public void Generate(TypeBuilder typeBuilder, string propertyName)
        {
            if(!typeBuilder.ImplementedInterfaces.Contains(typeof(INotifyPropertyChanged)))
                AddInterfaceImplementation(typeBuilder);

            var propertyInfo = typeBuilder.BaseType.GetProperties().Single(item => item.Name == propertyName);
            WrapMethod(propertyInfo, _propertyChangedMethodBuilder, typeBuilder);
        }

        private static void WrapMethod(PropertyInfo item, MethodBuilder raisePropertyChanged, TypeBuilder typeBuilder)
        {
            var setMethod = item.GetSetMethod();

            var types = from t in setMethod.GetParameters() select t.ParameterType;

            var setMethodBuilder = typeBuilder.DefineMethod(setMethod.Name, setMethod.Attributes, setMethod.ReturnType, types.ToArray());
            typeBuilder.DefineMethodOverride(setMethodBuilder, setMethod);
            var setMethodWrapperIl = setMethodBuilder.GetILGenerator();

            // base.[PropertyName] = value;
            setMethodWrapperIl.Emit(OpCodes.Ldarg_0);
            setMethodWrapperIl.Emit(OpCodes.Ldarg_1);
            setMethodWrapperIl.EmitCall(OpCodes.Call, setMethod, null);

            // RaisePropertyChanged("[PropertyName]");
            setMethodWrapperIl.Emit(OpCodes.Ldarg_0);
            setMethodWrapperIl.Emit(OpCodes.Ldstr, item.Name);
            setMethodWrapperIl.EmitCall(OpCodes.Call, raisePropertyChanged, null);

            // return;
            setMethodWrapperIl.Emit(OpCodes.Ret);
        }

        private static void AddInterfaceImplementation(TypeBuilder typeBuilder)
        {
            typeBuilder.AddInterfaceImplementation(typeof(INotifyPropertyChanged));
            var eventField = typeBuilder.DefineField("PropertyChanged", typeof(PropertyChangedEventHandler), FieldAttributes.Private);

            var addMethod = DefineAddPropertyChangedMethod(typeBuilder, eventField);
            var removeMethod = DefineRemovePropertyChangedMethod(typeBuilder, eventField);

            typeBuilder.DefineMethodOverride(addMethod, typeof(INotifyPropertyChanged).GetMethod("add_PropertyChanged"));
            typeBuilder.DefineMethodOverride(removeMethod, typeof(INotifyPropertyChanged).GetMethod("remove_PropertyChanged"));
            var eventBuilder = typeBuilder.DefineEvent(nameof(INotifyPropertyChanged.PropertyChanged), EventAttributes.None, typeof(ProgressChangedEventHandler));
            eventBuilder.SetAddOnMethod(addMethod);
            eventBuilder.SetRemoveOnMethod(removeMethod);

            _propertyChangedMethodBuilder = CreateRaisePropertyChanged(typeBuilder, eventField);
        }

        private static MethodBuilder CreateRaisePropertyChanged(TypeBuilder typeBuilder, FieldBuilder eventField)
        {
            var raisePropertyChangedBuilder =
                typeBuilder.DefineMethod(
                    "RaisePropertyChanged",
                    MethodAttributes.Family | MethodAttributes.Virtual,
                    null, new[] {typeof(string)});

            var raisePropertyChangedIl =
                raisePropertyChangedBuilder.GetILGenerator();
            var labelExit = raisePropertyChangedIl.DefineLabel();

            raisePropertyChangedIl.Emit(OpCodes.Ldarg_0);
            raisePropertyChangedIl.Emit(OpCodes.Ldfld, eventField);
            raisePropertyChangedIl.Emit(OpCodes.Ldnull);
            raisePropertyChangedIl.Emit(OpCodes.Ceq);
            raisePropertyChangedIl.Emit(OpCodes.Brtrue, labelExit);

            raisePropertyChangedIl.Emit(OpCodes.Ldarg_0);
            raisePropertyChangedIl.Emit(OpCodes.Ldfld, eventField);
            raisePropertyChangedIl.Emit(OpCodes.Ldarg_0);
            raisePropertyChangedIl.Emit(OpCodes.Ldarg_1);
            raisePropertyChangedIl.Emit(OpCodes.Newobj, typeof(PropertyChangedEventArgs).GetConstructor(new[] {typeof(string)}));
            raisePropertyChangedIl.EmitCall(OpCodes.Callvirt, typeof(PropertyChangedEventHandler).GetMethod("Invoke"), null);

            raisePropertyChangedIl.MarkLabel(labelExit);
            raisePropertyChangedIl.Emit(OpCodes.Ret);

            return raisePropertyChangedBuilder;
        }

        private static MethodBuilder DefineAddPropertyChangedMethod(TypeBuilder typeBuilder, FieldBuilder eventField)
        {
            var methodBuilder = typeBuilder.DefineMethod("add_PropertyChanged", MethodAttributes.Public | MethodAttributes.Virtual, null, new[] {typeof(PropertyChangedEventHandler)});
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Synchronized);
            var ilGen = methodBuilder.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, eventField);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.EmitCall(OpCodes.Call, typeof(Delegate).GetMethod(nameof(Delegate.Combine), new[] {typeof(Delegate), typeof(Delegate)}), null);
            ilGen.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
            ilGen.Emit(OpCodes.Stfld, eventField);
            ilGen.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        private static MethodBuilder DefineRemovePropertyChangedMethod(TypeBuilder typeBuilder, FieldInfo eventField)
        {
            var methodBuilder = typeBuilder.DefineMethod("remove_PropertyChanged", MethodAttributes.Public | MethodAttributes.Virtual, null, new[] {typeof(PropertyChangedEventHandler)});
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Synchronized);
            var ilGen = methodBuilder.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, eventField);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.EmitCall(OpCodes.Call, typeof(Delegate).GetMethod(nameof(Delegate.Remove), new[] {typeof(Delegate), typeof(Delegate)}), null);
            ilGen.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
            ilGen.Emit(OpCodes.Stfld, eventField);
            ilGen.Emit(OpCodes.Ret);
            return methodBuilder;
        }

        private static MethodBuilder _propertyChangedMethodBuilder;
    }
}