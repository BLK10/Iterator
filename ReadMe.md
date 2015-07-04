# Iterator #

Iterator is a coroutine library for Unity 3D.

## Asynchronous operations ##

One of the purpose of Iterator is to simplify asynchronous operations in Unity 3D and propose a better way to manage your coroutines.  
Unity provides iterator-based coroutine framework for asynchronous operation, but it suffers from some lack of functionality. It's slightly difficult to use Unity coroutine when you want:  
- to return a value.
- to cancel a coroutine.
- to chain multiple coroutine.
- to wait for multiple coroutine to complete.
- control the way your coroutine execute(Asynchrone, synchrone) without modifying the inner routine.
- ...and so on

So because of this, Iterator proposes to fill the gaps of Unity 3D coroutines.

This project was forked from [http://github.com/aiming/iterator-tasks](http://github.com/aiming/iterator-tasks) who uses the Task terminology.
But in my sense this is cumbersome, even if they share some concept coroutine aren't task.
This library reintegrate the coroutine keyword and while it keeps getting some Task lib api terminology some method and behaviour have changed, evolved.


## Dependancy ##

Iterator depends on:
- [the Singleton Toolbox.](http://github.com/BLK10/Singleton)
- [the Tuple.](http://github.com/BLK10/Collection)

## Example Usage ##

This library has been redesigned with one goal in mind: ["Keep it simple, stupid"](https://en.wikipedia.org/wiki/KISS_principle).

TODO


### [License remains to Aiming Inc](http://github.com/BLK10/Iterator/blob/master/LICENSE.md) ###

