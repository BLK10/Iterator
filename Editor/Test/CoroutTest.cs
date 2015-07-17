using System;
using System.Collections;

using BLK10.Iterator;
using BLK10.Report;

using UnityEditor;
using UnityEngine;

using NUnit.Framework;

namespace BLK10.Iterator.Test
{
    [TestFixture]
    [Category("Corout Test")]
    public class CoroutTest
    {

        [Test]
        public void Simple()
        {            
            var x = 7.525f;
            var y = x * x;
                        
            Corout.Create<float>(co => Routines.GetFloatSquareRoot(x, co))
                  .OnSucceed(co =>
                  {
                      Assert.AreEqual(co.Result, y);
                      Assert.IsTrue(co.Succeeded);                     
                  })
                  .Start();            
        }

        [Test]
        public void StepAndSecond()
        {
            int startStep = Scheduler.Instance.Step;
            int step      = 6;
                        
            Corout.Create(Routines.NStep(step))
                  .OnSucceed(co => Assert.AreEqual(Scheduler.Instance.Step, startStep + step))
                  .Start();


            float startSecond = Scheduler.Instance.Second;
            float second      = 0.5f;

            Corout.Create(Routines.NSecond(second))
                  .OnSucceed(co => Assert.GreaterOrEqual(Scheduler.Instance.Second, startSecond + second))
                  .Start();

        }

        [Test]
        public void CatchThrowFinally()
        {
            var x = 42;
            var y = x * x;

            Corout.Create<int>(co => Routines.Failing(x, co))
                  .OnCatch<NotSupportedException>((ex, co) =>
                  {
                      Assert.IsInstanceOf<NotSupportedException>(ex);
                      Debug.Log("Catch: 'NotSupportedException'.");
                  })
                  .OnFinally(co =>
                  {
                      Assert.IsTrue(co.Faulted); 
                      Debug.Log("Finally do some clean-up...");
                  })
                  .Start();


            Corout.Create<int>(co => Routines.GetIntSquareRoot(x, co))
                  .OnFinally(co =>
                  {
                      Debug.Log("'OnFinally' doesn't need 'OnCatch' and are always executed.");
                      Assert.AreEqual(co.Result, y);
                      Assert.IsTrue(co.Succeeded);                      
                  })
                  .Start();


            Corout.Create<int>(co => Routines.Failing(x, co))
                  .OnCatch<NotSupportedException>((ex, co) =>
                  {
                      Assert.IsInstanceOf<NotSupportedException>(ex);
                      Assert.IsTrue(co.Faulted);
                      throw ex;
                  })
                  .OnFinally(co =>
                  {
                      Assert.IsTrue(co.Faulted);
                      Debug.Log("Even if you throw exception 'OnFinally' is executed.");
                  })
                  .Start();
        }

        [Test]
        public void Cancel()
        {
            var x = 7.525f;

            Corout.Create<float>((co, tk) => Routines.Cancelable(x, co, tk))                  
                  .OnSucceed(co => Debug.Log("OnSucceed."))
                  .OnCancel(co => Debug.Log("Corout canceled."))
                  .Start();

            var second = 1.15f;
            Corout.Create<float>((co, tk) => Routines.CancelableAfter(second, x, co, tk))
                  .OnCancel(co => Debug.Log(string.Format("Corout canceled after {0} second", second)))
                  .OnSucceed(co => Debug.Log("OnSucceed."))
                  .Start();

            var step = 15;
            Corout.Create<float>((co, tk) => Routines.CancelableAfter(step, x, co, tk))
                  .OnSucceed(co => Debug.Log("'OnSucceed' never call."))
                  .OnCancel(co => Debug.Log(string.Format("Corout canceled after {0} step", step)))
                  .Start();
        }

        [Test]
        public void Abort()
        {
            var step = 100;

            var corout = Corout.Create(Routines.NStepVerbose(step))
                               .OnSucceed(co => Debug.Log(string.Format("corout complete in {0} steps", step)))
                               .OnCancel( co => Debug.Log("'OnCancel' never called"))
                               .OnFinally(co => Debug.Log("'OnFinally'."));

            corout.Start();
            
            
            Corout.WaitThen(10, () =>
            {                
                corout.Abort();
                Debug.Log("Aborted");
            });
            
            // OR
            /*
            Corout.WaitThen(0.5f, () =>
            {                
                corout.Abort();
                Debug.Log("Aborted");
            });
            */
        }

        [Test]
        public void Fence()
        {
            var first = 10;
            var second = 05;
            var third = 03;

            int startStep = Scheduler.Instance.Step;

            using (Corout.OpenFence(() => Debug.Log("fenced corout ends...")))
            {
                Corout.Create(() => Routines.NStepVerbose(first))
                      .OnSucceed(co =>
                      {
                          Debug.Log(string.Format("first corout complete in {0} steps", first));
                          Assert.AreEqual(Scheduler.Instance.Step, startStep + first);
                      })
                      .Start();

                Corout.Create(() => Routines.NStepVerbose(second))
                      .OnSucceed(co =>
                      {
                          Debug.Log(string.Format("second corout Complete in {0} steps", second));
                          Assert.AreEqual(Scheduler.Instance.Step, startStep + second);
                      })
                      .Start();
            }

            Corout.Create(() => Routines.NStepVerbose(third))
                  .OnSucceed(co =>
                  {
                      Debug.Log(string.Format("third corout complete in {0} steps", third));
                      Assert.AreEqual(Scheduler.Instance.Step, startStep + Math.Max(first, second) + third + 1); // 1 step for fence callback
                  })
                  .Start();

            // OR
            /*
            var fence01 = Corout.OpenFence(() => Debug.Log("fenced corout ends..."));

            Corout.Create(() => Routines.NStepVerbose(first))
                  .OnComplete(co =>
                  {
                      Debug.Log(string.Format("first corout complete in {0} steps", first));
                      Assert.AreEqual(Corout.CurrentStep, startStep + first);
                  })
                  .Start();

            Corout.Create(() => Routines.NStepVerbose(second))
                  .OnComplete(co =>
                  {
                      Debug.Log(string.Format("second corout Complete in {0} steps", second));
                      Assert.AreEqual(Corout.CurrentStep, startStep + second);
                  })
                  .Start();
            
            fence01.Close();

            Corout.Create(() => Routines.NStepVerbose(third))
                  .OnComplete(co =>
                  {
                      Debug.Log(string.Format("third corout complete in {0} steps", third));
                      Assert.AreEqual(Corout.CurrentStep, startStep + Math.Max(first, second) + third + 1); // 1 step for fence callback
                  })
                  .Start();
            */

        }
        


        [Test]
        public void ContinueSimple()
        {
            var x = 7.525f;
            var y = x * x;
            var s = y.ToString();
            int l = s.Length;
                        
            Corout.Create<float>(co => Routines.GetFloatSquareRoot(x, co))
                  .OnSucceed(co => Assert.AreEqual(co.Result, y))
                  .ContinueWith<string>(Routines.GetFloatString)
                  .OnSucceed(co => Assert.AreEqual(co.Result, s))
                  .ContinueWith<int>(Routines.GetLength)
                  .OnSucceed(co => Assert.AreEqual(co.Result, l))
                  .Start();            
        }
        
        [Test]
        public void Interlace()
        {
            var x = 3;
            var y = 4.57f;
            var z = 1425.67f;

            var xx = x * x;
            var yy = y * y;
            var zz = z.ToString();

            var cw = string.Join(" ", new string[] { xx.ToString(), yy.ToString(), zz });
            

            Corout.WhenAll(() => Routines.NStepVerbose(5),
                           () => Routines.NStepVerbose(10),
                           () => Routines.NStepVerbose(15))
                  .OnSucceed(co =>
                  {
                      Debug.Log("OnSucceed");
                  })                  
                  .Start();


            Corout.WhenAll<int, float, string>(co => Routines.GetIntSquareRoot(x, co),
                                               co => Routines.GetFloatSquareRoot(y, co),
                                               co => Routines.GetFloatString(z, co))
                  .OnSucceed(co =>
                  {
                      Debug.Log(co.Result.Item1);
                      Debug.Log(co.Result.Item2);
                      Debug.Log(co.Result.Item3);
                  })
                  .ContinueWith<string>(Routines.JoinString)
                  .OnSucceed(co =>
                  {
                      Debug.Log(co.Result);
                      Assert.AreEqual(cw, co.Result);
                  })
                  .Start();

        }

        [Test]
        public void Error02()
        {
            int x = 15;            
            var y = x * x;
            var s = y.ToString();
            int l = s.Length;

            Corout.Create<int>(co => Routines.Failing(x, co))
                  .OnCatch<NotSupportedException>((ex, co) =>
                  {
                      Debug.Log("Catch error.");
                      co.Abort();
                  })
                  .OnSucceed(co =>
                  {
                      Debug.Log("first on succeed.");
                      Assert.AreEqual(co.Result, y);
                  })
                  .ContinueWith<string>(Routines.GetIntString)
                  .OnSucceed(co =>
                  {
                      Debug.Log("second on succeed should not happen.");
                      Assert.AreEqual(co.Result, s);
                  })
                  .ContinueWith<int>(Routines.GetLength)
                  .OnSucceed(co =>
                  {
                      Debug.Log("third on succeed should not happen.");
                      Assert.AreEqual(co.Result, l);
                  })
                  .Start();

        }


        [Test]
        public void InterlaceError()
        {
            var x = 3;
            var y = 4.57f;
            var z = 1425.67f;

            var xx = x * x;
            var yy = y * y;
            var zz = z.ToString();

            var cw = string.Join(" ", new string[] { xx.ToString(), yy.ToString(), zz });

            Corout.WhenAll<int, float, string>(ac => Routines.Failing(x, ac),
                                               ac => Routines.GetFloatSquareRoot(y, ac),
                                               ac => Routines.GetFloatString(z, ac))
                  .OnCatch<Exception>((ex, co) =>
                  {
                      Debug.Log("catch error");
                      //throw ex;
                      co.Abort();
                  })
                  .ContinueWith(() => Routines.NStepVerbose(10))
                  //.ContinueWith<string>(Routines.JoinString)
                  //.OnComplete(co =>
                  //{

                      //Debug.Log(co.Result);
                      //Assert.AreEqual(cw, co.Result);
                  //})
                  .Start();
            
            
        }
                
    }

    [TestFixture]
    [Category("Command Test")]
    public class CommandTest
    {

    }
}
