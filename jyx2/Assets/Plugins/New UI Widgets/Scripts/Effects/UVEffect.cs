namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// UV effect.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(Graphic))]
	public abstract class UVEffect : BaseMeshEffect, IMaterialModifier, IMeshModifier
	{
		/// <summary>
		/// UV channel.
		/// </summary>
		public enum UVChannel
		{
			/// <summary>
			/// Use UV0.zw.
			/// </summary>
			UV0zw = 0,

			/// <summary>
			/// Use UV1.xy.
			/// </summary>
			UV1xy = 1,
		}

		/// <summary>
		/// UV mode.
		/// </summary>
		public enum UVMode
		{
			/// <summary>
			/// UV0.y limited by width.
			/// x in [0, 1]; y in [0, 1 / (width / height)]
			/// </summary>
			X = 0,

			/// <summary>
			/// UV0.x limited by height.
			/// x in [0, 1 / (height / width)]; y in [0, 1]
			/// </summary>
			Y = 1,

			/// <summary>
			/// UV0.x is not limited by size.
			/// x in [0, 1]; y in [0, 1]
			/// </summary>
			One = 2,

			/// <summary>
			/// If width more that height then works as X mode; otherwise Y mode.
			/// </summary>
			Max = 3,
		}

		/// <summary>
		/// Mesh sizes.
		/// </summary>
		protected struct MeshSize
		{
			/// <summary>
			/// Minimal x and y values.
			/// </summary>
			public readonly Vector2 Min;

			/// <summary>
			/// Maximal x and y values.
			/// </summary>
			public readonly Vector2 Max;

			/// <summary>
			/// Width.
			/// </summary>
			public readonly float Width;

			/// <summary>
			/// Height.
			/// </summary>
			public readonly float Height;

			/// <summary>
			/// Aspect ratio.
			/// </summary>
			public readonly Vector2 AspectRatio;

			/// <summary>
			/// Initializes a new instance of the <see cref="MeshSize"/> struct.
			/// </summary>
			/// <param name="vh">Vertex helper.</param>
			/// <param name="size">RectTransform size.</param>
			/// <param name="mode">UV mode.</param>
			public MeshSize(VertexHelper vh, Vector2 size, UVMode mode)
			{
				var vertex = default(UIVertex);

				// get min and max position to calculate uv1
				vh.PopulateUIVertex(ref vertex, 0);
				Min = new Vector2(vertex.position.x, vertex.position.y);
				Max = Min;

				for (int i = 1; i < vh.currentVertCount; i++)
				{
					vh.PopulateUIVertex(ref vertex, i);

					Min.x = Math.Min(Min.x, vertex.position.x);
					Min.y = Math.Min(Min.y, vertex.position.y);

					Max.x = Math.Max(Max.x, vertex.position.x);
					Max.y = Math.Max(Max.y, vertex.position.y);
				}

				// size
				Width = Max.x - Min.x;
				Height = Max.y - Min.y;

				// calculate aspect ratio
				switch (mode)
				{
					case UVMode.X:
						AspectRatio = new Vector2(1f, size.x / size.y);
						break;
					case UVMode.Y:
						AspectRatio = new Vector2(size.y / size.x, 1f);
						break;
					case UVMode.One:
						AspectRatio = Vector2.one;
						break;
					case UVMode.Max:
						AspectRatio = (size.x >= size.y)
							? new Vector2(1f, size.x / size.y)
							: new Vector2(size.y / size.x, 1f);
						break;
					default:
						throw new NotSupportedException(string.Format("Unknown UVMode: {0}", EnumHelper<UVMode>.ToString(mode)));
				}
			}
		}

		/// <summary>
		/// Shader.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("RippleShader")]
		protected Shader EffectShader;

		/// <summary>
		/// Base material.
		/// </summary>
		[NonSerialized]
		protected Material BaseMaterial;

		/// <summary>
		/// Ring material.
		/// </summary>
		[NonSerialized]
		protected Material EffectMaterial;

		[NonSerialized]
		RectTransform rectTransform;

		/// <summary>
		/// RectTransform component.
		/// </summary>
		protected RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		UVChannel channel = UVChannel.UV0zw;

		/// <summary>
		/// UV channel
		/// </summary>
		protected UVChannel Channel
		{
			get
			{
				return channel;
			}

			set
			{
				if (channel != value)
				{
					channel = value;
					graphic.SetVerticesDirty();
				}
			}
		}

		UVMode mode = UVMode.X;

		/// <summary>
		/// UV1 mode.
		/// </summary>
		protected UVMode Mode
		{
			get
			{
				return mode;
			}

			set
			{
				if (mode != value)
				{
					mode = value;
					graphic.SetVerticesDirty();
				}
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (graphic != null)
			{
				if ((graphic.canvas != null) && !IsSet(graphic.canvas.additionalShaderChannels, AdditionalCanvasShaderChannels.TexCoord1))
				{
					graphic.canvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;
				}

				graphic.SetVerticesDirty();
			}
		}

		/// <summary>
		/// Is channels has specified channel?
		/// </summary>
		/// <param name="channels">Channels.</param>
		/// <param name="channel">Channel.</param>
		/// <returns>true if specified channel is set; otherwise false.</returns>
		protected bool IsSet(AdditionalCanvasShaderChannels channels, AdditionalCanvasShaderChannels channel)
		{
			return (channels & channel) == channel;
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			if (graphic != null)
			{
				graphic.SetMaterialDirty();
			}

			base.OnDisable();
		}

		/// <summary>
		/// Process the animation event.
		/// </summary>
		protected override void OnDidApplyAnimationProperties()
		{
			if (graphic != null)
			{
				graphic.SetMaterialDirty();
			}

			base.OnDidApplyAnimationProperties();
		}

#if UNITY_EDITOR

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (graphic != null)
			{
				UpdateMaterial();
			}
		}

#endif

		/// <summary>
		/// Process the dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			base.OnRectTransformDimensionsChange();

			UpdateMaterial();
		}

		/// <summary>
		/// Add uv1 to the mesh.
		/// </summary>
		/// <param name="vh">Vertex helper.</param>
		public override void ModifyMesh(VertexHelper vh)
		{
			var size = RectTransform.rect.size;
			if ((size.x < 0f) || (size.y < 0f))
			{
				return;
			}

			var vertex = default(UIVertex);
			var mesh_size = new MeshSize(vh, RectTransform.rect.size, Mode);

			switch (Channel)
			{
				case UVChannel.UV0zw:
					for (int i = 0; i < vh.currentVertCount; i++)
					{
						vh.PopulateUIVertex(ref vertex, i);

						vertex.uv0 = new Vector4(
							vertex.uv0.x,
							vertex.uv0.y,
							(vertex.position.x - mesh_size.Min.x) / mesh_size.Width / mesh_size.AspectRatio.x,
							(vertex.position.y - mesh_size.Min.y) / mesh_size.Height / mesh_size.AspectRatio.y);

						vh.SetUIVertex(vertex, i);
					}

					break;
				case UVChannel.UV1xy:
					for (int i = 0; i < vh.currentVertCount; i++)
					{
						vh.PopulateUIVertex(ref vertex, i);

						vertex.uv1 = new Vector2(
							(vertex.position.x - mesh_size.Min.x) / mesh_size.Width / mesh_size.AspectRatio.x,
							(vertex.position.y - mesh_size.Min.y) / mesh_size.Height / mesh_size.AspectRatio.y);

						vh.SetUIVertex(vertex, i);
					}

					break;
				default:
					throw new NotSupportedException(string.Format("Unknown Channel: {0}", EnumHelper<UVChannel>.ToString(Channel)));
			}
		}

		/// <summary>
		/// Init material.
		/// </summary>
		protected virtual void InitMaterial()
		{
			SetMaterialProperties();
		}

		/// <summary>
		/// Set material properties.
		/// </summary>
		protected abstract void SetMaterialProperties();

		/// <summary>
		/// Update material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			SetMaterialProperties();

			if (EffectMaterial != null)
			{
				graphic.SetMaterialDirty();
			}
		}

		/// <summary>
		/// Get modified material.
		/// </summary>
		/// <param name="newBaseMaterial">Base material.</param>
		/// <returns>Modified material.</returns>
		public virtual Material GetModifiedMaterial(Material newBaseMaterial)
		{
			if ((BaseMaterial != null) && (newBaseMaterial.GetInstanceID() == BaseMaterial.GetInstanceID()))
			{
				return EffectMaterial;
			}

			if (EffectMaterial != null)
			{
#if UNITY_EDITOR
				DestroyImmediate(EffectMaterial);
#else
				Destroy(EffectMaterial);
#endif
			}

			BaseMaterial = newBaseMaterial;
			EffectMaterial = new Material(newBaseMaterial)
			{
				shader = EffectShader,
			};
			InitMaterial();

			return EffectMaterial;
		}
	}
}