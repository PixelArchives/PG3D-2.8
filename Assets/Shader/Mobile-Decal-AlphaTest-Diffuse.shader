Shader "Decal/Mobile/Cutout Diffuse" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
 _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "LIGHTMODE"="Vertex" "QUEUE"="Transparent" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  Lighting On
  Material {
   Ambient [_Color]
   Diffuse [_Color]
  }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  AlphaTest Greater 0
  ColorMask RGB
  Offset -1, -1
  SetTexture [_MainTex] { combine texture * primary double, texture alpha * primary alpha }
 }
}
Fallback "Decal/Cutout VertexLit"
}