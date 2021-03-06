﻿using OpenTK.Graphics.OpenGL4;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.Shaders
{
    [Combinator]
    [WorkflowElementCategory(ElementCategory.Sink)]
    [Description("Binds a texture buffer to the specified texture unit.")]
    public class BindTexture
    {
        public BindTexture()
        {
            TextureSlot = TextureUnit.Texture0;
            TextureTarget = TextureTarget.Texture2D;
        }

        [Description("The slot on which to bind the texture.")]
        public TextureUnit TextureSlot { get; set; }

        [TypeConverter(typeof(ShaderNameConverter))]
        [Description("The name of the shader program.")]
        public string ShaderName { get; set; }

        [TypeConverter(typeof(TextureNameConverter))]
        [Description("The optional name of the texture that will be bound to the shader.")]
        public string TextureName { get; set; }

        [Description("The texture target that will be bound to the sampler.")]
        public TextureTarget TextureTarget { get; set; }

        [Description("The optional index of the texture that will be bound to the shader. Only applicable to texture array objects.")]
        public int? Index { get; set; }

        [Browsable(false)]
        public bool IndexSpecified
        {
            get { return Index.HasValue; }
        }

        IObservable<TSource> Process<TSource>(IObservable<TSource> source, Action<TSource> update)
        {
            return Observable.Defer(() =>
            {
                var texture = default(Texture);
                var textureName = default(string);
                return source.CombineEither(
                    ShaderManager.ReserveShader(ShaderName),
                    (input, shader) =>
                    {
                        if (textureName != TextureName)
                        {
                            textureName = TextureName;
                            texture = !string.IsNullOrEmpty(textureName)
                                ? shader.Window.ResourceManager.Load<Texture>(textureName)
                                : null;
                        }

                        if (texture != null)
                        {
                            var index = Index;
                            var textureId = index.HasValue ? ((TextureSequence)texture).Textures[index.Value] : texture.Id;
                            shader.Update(() =>
                            {
                                GL.ActiveTexture(TextureSlot);
                                GL.BindTexture(TextureTarget, textureId);
                            });
                        }
                        else if (update != null) shader.Update(() => update(input));
                        return input;
                    });
            });
        }

        public IObservable<TSource> Process<TSource>(IObservable<TSource> source)
        {
            return Process(source, update: null);
        }

        public IObservable<Texture> Process(IObservable<Texture> source)
        {
            return Process(source, input =>
            {
                GL.ActiveTexture(TextureSlot);
                GL.BindTexture(TextureTarget, input != null ? input.Id : 0);
            });
        }
    }
}
