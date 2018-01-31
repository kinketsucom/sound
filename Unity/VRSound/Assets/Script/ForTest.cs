using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ForTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SumTest(){

		int sum = 0;
		Static.check_time = Time.realtimeSinceStartup;
//		for (int i = 0; i < 100000000; i++) {
//			sum += 1;
//		}
//		print (sum.ToString()+"for");

//		Static.check_time = Time.realtimeSinceStartup - Static.check_time;
//		Debug.Log( "check time : " + Static.check_time.ToString("0.00000") );


//		Parallel.For (0, 10000, j => {
//			Interlocked.Add(ref sum, 1);
//		});
//		print (sum.ToString () + "para");

//		var data = Enumerable.Range(0, 100);
//		var sqSum = data.AsParallel().Sum((x) => 1);
//		print(sqSum);

//		int[] hoge = new int[640];
//		var sqSum = hoge.AsParallel().Sum((x)=>x);
//		print(sqSum);
//		int nums = 100000000;
//		long total = 0;
//
//		// Use type parameter to make subtotal a long, not an int
//		Parallel.For<long>(0,nums,() => 0,(j, loop, subtotal) =>
//			{
//				subtotal += 1;
//				return subtotal;
//			},
//			(x) => Interlocked.Add(ref total, x)
//		);
//
//		print(total);



		Static.check_time = Time.realtimeSinceStartup - Static.check_time;
		Debug.Log( "check time : " + Static.check_time.ToString("0.00000") );
	}
}
