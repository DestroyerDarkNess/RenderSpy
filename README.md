
<h1 align="center">RenderSpy</h1>
<p align="center">
  <a href="https://github.com/DestroyerDarkNess/RenderSpy/blob/master/LICENSE">
    <img src="https://img.shields.io/github/license/Rebzzel/kiero.svg?style=flat-square"/>
  </a>
   <img src="https://img.shields.io/badge/platform-Windows-0078d7.svg"/>
  <br>
  Universal graphical and input hook for a D3D9-D3D12, OpenGL and Vulkan based games
</p>

# Features
The objectives of the list that have: [❌] - It means I have no idea how to solve it.

### Graphics

- [x] DirectX9 
- [x] DirectX10
- [x] DirectX11 
- [ ] DirectX12  
- [x] OpenGL    
- [ ] Vulkan. ❌ **[I don't know how to hook it]**

### Inputs

- [x] GetRawInputData 
- [x] DefWindowProc
- [x] GetWindowLongPtr
- [x] SetWindowLongPtr 
- [x] MouseCursorHook 

### Example

```C
   if (WinApi.GetModuleHandle("d3d9.dll") != IntPtr.Zero)
                {
                    Graphics.d3d9.EndScene NewEndScene = new Graphics.d3d9.EndScene();
                    NewEndScene.Install();

                    NewEndScene.EndSceneEvent += (device) =>
                    {
                        Console.WriteLine("d3d9 EndScene Hooked! Device Address: " + device.ToString());
                        return 0;
                    };
                }
                else if (WinApi.GetModuleHandle("d3d10.dll") != IntPtr.Zero)
                {
                    Graphics.d3d10.Present NewPresent = new Graphics.d3d10.Present();
                    NewPresent.Install();

                    NewPresent.PresentEvent += (swapChainPtr, syncInterval, flags) =>
                    {
                        Console.WriteLine("d3d10 Present Hooked! swapChainPtr Address: " + swapChainPtr.ToString());
                        return 0;
                    };
                }
                else if (WinApi.GetModuleHandle("d3d11.dll") != IntPtr.Zero)
                {
                    Graphics.d3d11.Present NewPresent = new Graphics.d3d11.Present();
                    NewPresent.Install();

                    NewPresent.PresentEvent += (swapChainPtr, syncInterval, flags) =>
                    {
                        Console.WriteLine("d3d11 Present Hooked! swapChainPtr Address: " + swapChainPtr.ToString());
                        return 0;
                    };
                }
                else if (WinApi.GetModuleHandle("d3d12.dll") != IntPtr.Zero)
                {
                    // No Implement!!
                }
                else if (WinApi.GetModuleHandle("opengl32.dll") != IntPtr.Zero)
                {
                    Graphics.opengl.wglSwapBuffers NewSwapBuffers = new Graphics.opengl.wglSwapBuffers();
                    NewSwapBuffers.Install();

                    NewSwapBuffers.wglSwapBuffersEvent += (hdc) =>
                    {
                        Console.WriteLine("OpenGL wglSwapBuffers Hooked! SwapBuffers Address: " + hdc.ToString());
                        return true;
                    };
                }
```

### License
```
MIT License

Copyright (c) 2019-2023 DestroyerDarkNess

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```






