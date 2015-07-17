from [http://github.com/aiming/iterator-tasks](http://github.com/aiming/iterator-tasks)


# Iterator #

Iterator is a coroutine library for Unity 3D.


## Asynchronous operations ##

One of the purpose of Iterator is to provide complete asynchronous operations in Unity.  
Unity provides iterator based coroutine methods for asynchronous operation, but it suffers from some lack of functionality. It's slightly difficult to use Unity coroutine when you want:  
- to return a value.
- to cancel a coroutine.
- to chain multiple coroutine.
- to wait for multiple coroutine to complete.
- to control the way your coroutine execute(Asynchronous, synchronous).
- ...and so on

So because of this, Iterator proposes to fill the gaps of Unity 3D coroutines.

This project was forked from [iterator-tasks](http://github.com/aiming/iterator-tasks) who uses the System.Threading.Task terminology.
This library reintegrate the coroutine semantic and while it keeps getting some Task api terminology some method and behaviour have greatly changed / evolved.


## Dependancy ##

Iterator depends on:
- [the Singleton Toolbox.](http://github.com/BLK10/Singleton)
- [the Tuple.](http://github.com/BLK10/Collection)


## Example Usage ##

This library has been redesigned with one goal in mind: ["Keep it simple, stupid"](https://en.wikipedia.org/wiki/KISS_principle)... really!

A samples of methods are in the 'CoroutTest' class which you can find [>> here <<]()
The 'Routines' methods are not representative of the workload and their purpose is to illustrate the syntax.

----------
### [License remains to Aiming Inc](http://github.com/BLK10/Iterator/blob/master/LICENSE.md) ###

