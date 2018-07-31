using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameFrameWork
{
    /// <summary>
    /// Shader Property 属性的类型(对应Editor 下ShaderUtil.ShaderPropertyType )
    /// </summary>
    public enum ShaderPropertyEnum
    {
        Color = 0,
        Vector = 1,
        Float = 2,
        Range = 3,
        TexEnv = 4
    }
}