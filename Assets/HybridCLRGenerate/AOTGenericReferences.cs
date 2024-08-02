using System.Collections.Generic;
public class AOTGenericReferences : UnityEngine.MonoBehaviour
{

	// {{ AOT assemblies
	public static readonly IReadOnlyList<string> PatchedAOTAssemblyList = new List<string>
	{
		"UnityEngine.AndroidJNIModule.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
	};
	// }}

	// {{ constraint implement type
	// }} 

	// {{ AOT generic types
	// System.Action<byte,object>
	// System.Action<byte>
	// System.Action<object,object>
	// System.Action<object>
	// System.ByReference<UnityEngine.jvalue>
	// System.Collections.Generic.ArraySortHelper<byte>
	// System.Collections.Generic.ArraySortHelper<object>
	// System.Collections.Generic.Comparer<byte>
	// System.Collections.Generic.Comparer<object>
	// System.Collections.Generic.Dictionary.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.KeyCollection<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection.Enumerator<object,object>
	// System.Collections.Generic.Dictionary.ValueCollection<object,object>
	// System.Collections.Generic.Dictionary<object,object>
	// System.Collections.Generic.EqualityComparer<object>
	// System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.ICollection<byte>
	// System.Collections.Generic.ICollection<object>
	// System.Collections.Generic.IComparer<byte>
	// System.Collections.Generic.IComparer<object>
	// System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerable<byte>
	// System.Collections.Generic.IEnumerable<object>
	// System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<object,object>>
	// System.Collections.Generic.IEnumerator<byte>
	// System.Collections.Generic.IEnumerator<object>
	// System.Collections.Generic.IEqualityComparer<object>
	// System.Collections.Generic.IList<byte>
	// System.Collections.Generic.IList<object>
	// System.Collections.Generic.KeyValuePair<object,object>
	// System.Collections.Generic.List.Enumerator<byte>
	// System.Collections.Generic.List.Enumerator<object>
	// System.Collections.Generic.List<byte>
	// System.Collections.Generic.List<object>
	// System.Collections.Generic.ObjectComparer<byte>
	// System.Collections.Generic.ObjectComparer<object>
	// System.Collections.Generic.ObjectEqualityComparer<object>
	// System.Collections.ObjectModel.ReadOnlyCollection<byte>
	// System.Collections.ObjectModel.ReadOnlyCollection<object>
	// System.Comparison<byte>
	// System.Comparison<object>
	// System.Func<System.Threading.Tasks.VoidTaskResult>
	// System.Func<int>
	// System.Func<object,System.Threading.Tasks.VoidTaskResult>
	// System.Func<object,int>
	// System.Func<object,object,object>
	// System.Func<object>
	// System.Predicate<byte>
	// System.Predicate<object>
	// System.ReadOnlySpan<UnityEngine.jvalue>
	// System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable.ConfiguredTaskAwaiter<int>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.ConfiguredTaskAwaitable<int>
	// System.Runtime.CompilerServices.TaskAwaiter<System.Threading.Tasks.VoidTaskResult>
	// System.Runtime.CompilerServices.TaskAwaiter<int>
	// System.Span<UnityEngine.jvalue>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.ContinuationTaskFromResultTask<int>
	// System.Threading.Tasks.Task<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.Task<int>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory.<>c__DisplayClass35_0<int>
	// System.Threading.Tasks.TaskFactory<System.Threading.Tasks.VoidTaskResult>
	// System.Threading.Tasks.TaskFactory<int>
	// }}

	public void RefMethods()
	{
		// object[] System.Array.Empty<object>()
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TcpLoginClient.<SynGetChatRoomInfo>d__16>(System.Runtime.CompilerServices.TaskAwaiter&,TcpLoginClient.<SynGetChatRoomInfo>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<int>,TcpLoginClient.<ListenForMessages>d__20>(System.Runtime.CompilerServices.TaskAwaiter<int>&,TcpLoginClient.<ListenForMessages>d__20&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TcpLoginClient.<SynGetChatRoomInfo>d__16>(System.Runtime.CompilerServices.TaskAwaiter&,TcpLoginClient.<SynGetChatRoomInfo>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder<System.Threading.Tasks.VoidTaskResult>.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter<int>,TcpLoginClient.<ListenForMessages>d__20>(System.Runtime.CompilerServices.TaskAwaiter<int>&,TcpLoginClient.<ListenForMessages>d__20&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<TcpLoginClient.<ListenForMessages>d__20>(TcpLoginClient.<ListenForMessages>d__20&)
		// System.Void System.Runtime.CompilerServices.AsyncTaskMethodBuilder.Start<TcpLoginClient.<SynGetChatRoomInfo>d__16>(TcpLoginClient.<SynGetChatRoomInfo>d__16&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TcpLoginClient.<GetChatRoomInfo>d__27>(System.Runtime.CompilerServices.TaskAwaiter&,TcpLoginClient.<GetChatRoomInfo>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.AwaitUnsafeOnCompleted<System.Runtime.CompilerServices.TaskAwaiter,TcpLoginClient.<ListenMessage>d__25>(System.Runtime.CompilerServices.TaskAwaiter&,TcpLoginClient.<ListenMessage>d__25&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<TcpLoginClient.<GetChatRoomInfo>d__27>(TcpLoginClient.<GetChatRoomInfo>d__27&)
		// System.Void System.Runtime.CompilerServices.AsyncVoidMethodBuilder.Start<TcpLoginClient.<ListenMessage>d__25>(TcpLoginClient.<ListenMessage>d__25&)
		// int UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<int>(System.IntPtr)
		// object UnityEngine.AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<int>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine.AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// int UnityEngine.AndroidJavaObject.Call<int>(string,object[])
		// object UnityEngine.AndroidJavaObject.Call<object>(string,object[])
		// int UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<int>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject.FromJavaArrayDeleteLocalRef<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject.Get<object>(string)
		// object UnityEngine.AndroidJavaObject.GetStatic<object>(string)
		// int UnityEngine.AndroidJavaObject._Call<int>(System.IntPtr,object[])
		// int UnityEngine.AndroidJavaObject._Call<int>(string,object[])
		// object UnityEngine.AndroidJavaObject._Call<object>(System.IntPtr,object[])
		// object UnityEngine.AndroidJavaObject._Call<object>(string,object[])
		// object UnityEngine.AndroidJavaObject._Get<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject._Get<object>(string)
		// object UnityEngine.AndroidJavaObject._GetStatic<object>(System.IntPtr)
		// object UnityEngine.AndroidJavaObject._GetStatic<object>(string)
		// object UnityEngine.GameObject.AddComponent<object>()
		// object UnityEngine.ScriptableObject.CreateInstance<object>()
		// int UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<int>(System.IntPtr)
		// object UnityEngine._AndroidJNIHelper.ConvertFromJNIArray<object>(System.IntPtr)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetFieldID<object>(System.IntPtr,string,bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<int>(System.IntPtr,string,object[],bool)
		// System.IntPtr UnityEngine._AndroidJNIHelper.GetMethodID<object>(System.IntPtr,string,object[],bool)
		// string UnityEngine._AndroidJNIHelper.GetSignature<int>(object[])
		// string UnityEngine._AndroidJNIHelper.GetSignature<object>(object[])
	}
}