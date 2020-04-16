/**
Copyright (c)  2019, Francisco Xavier Dos Santos Fonseca (Ordem dos Engenheiros n.º 84598), and Technical University of Delft. 
All rights reserved. 

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met: 

1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer. 

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution. 

3. All advertising materials mentioning features or use of this software must display the following acknowledgement: 
This product includes software developed by the Technical University of Delft. 

4. Neither the name of  the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY  COPYRIGHT HOLDER "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL  COPYRIGHT HOLDER BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/



using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace SotS 
{
public class ConcurrentQueue<T>  {

	private readonly object syncLock = new object();
	private Queue<T> queue;

	public int Count
	{
		get
		{
			lock(syncLock) 
			{
				return queue.Count;
			}
		}
	}

	public ConcurrentQueue()
	{
		this.queue = new Queue<T>();
	}

	public T Peek()
	{
		lock(syncLock)
		{
			return queue.Peek();
		}
	}	

	public void Enqueue(T obj)
	{
		lock(syncLock)
		{
			queue.Enqueue(obj);
		}
	}

	public T Dequeue()
	{
		lock(syncLock)
		{
			return queue.Dequeue();
		}
	}

	public void Clear()
	{
		lock(syncLock)
		{
			queue.Clear();
		}
	}

	public T[] CopyToArray()
	{
		lock(syncLock)
		{
			if(queue.Count == 0)
			{
				return new T[0];
			}

			T[] values = new T[queue.Count];
			queue.CopyTo(values, 0);	
			return values;
		}
	}

	public static ConcurrentQueue<T> InitFromArray(IEnumerable<T> initValues)
	{
		var queue = new ConcurrentQueue<T>();

		if(initValues == null)	
		{
			return queue;
		}

		foreach(T val in initValues)
		{
			queue.Enqueue(val);
		}

		return queue;
	}
}

}