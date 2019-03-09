using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class CellFaceTest
    {
        [Test]
        public void CellFaceTestSimplePasses()
        {
            for (int x = 0; x < 6; x++)
                for (int y = 0; y < 36; y++)
                {
                    Debug.Log(Vector3.Dot(CellFace.DIRECTION[y], CellFace.FACES[x]));
                }
        }
    }
}
