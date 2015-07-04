Forked from [http://github.com/aiming/iterator-tasks](http://github.com/aiming/iterator-tasks)

# Iterator

Iterator is a coroutine library for Unity 3D.

## Asynchronous Operations on Unity

One of the purpose of Iterator is to simplify asynchronous operations in Unity 3D and propose a better way to manage your coroutines. Unity provides iterator-based coroutine framework for asynchronous operation but it suffers from some lack of functionality.  
For example, the following code awaits web access completion without blocking, by using a yield return statement.


    using UnityEngine;
    using System.Collections;
    
    public class example : MonoBehaviour
    {
    	public string url = "http://images.earthcam.com/ec_metros/ourcams/fridays.jpg";
    
    	IEnumerator Start()
    	{
    		using (WWW www = new WWW(url))
    		{
    			do
    			{
    				yield return null;
    			}
    			while (!www.isDone && www.error.IsNullOrEmpty());
    
    			renderer.material.mainTexture = www.texture;
    		}
    	}
    }


However, it is slightly difficult to use Unity coroutine when:  
- You want to return a value.
- You want to cancel a coroutine.
- You want to chain multiple coroutine.
- You want to wait for multiple coroutine to complete.
- You want to control the way your coroutine execute(Asynchrone, synchrone) without changing the 'inner' code.
- ...

So because of this, Iterator proposes to fill the gaps of Unity coroutines.

## Example Usage

TODO

# [License](http://github.com/BLK10/Iterator/blob/master/LICENSE.md)
