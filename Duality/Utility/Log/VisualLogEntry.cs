﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Duality.Drawing;

namespace Duality
{
	/// <summary>
	/// Represents an entry of a <see cref="VisualLog"/>, which will be generated by calling one of the 
	/// available drawing methods on one of the available logs.
	/// </summary>
	public abstract class VisualLogEntry
	{
		/// <summary>
		/// Default width of the outline of a log entries visual representation.
		/// </summary>
		protected	const	float	DefaultOutlineWidth	= 1.5f;
		private		const	float	LifeTimeEpsilon		= 0.000001f;

		private	float			maxLifetime			= LifeTimeEpsilon;
		private	float			lifetime			= LifeTimeEpsilon;
		private	ColorRgba		color				= ColorRgba.White;
		private bool            lifetimeAsAlpha		= false;   
		private	VisualLogAnchor	anchor				= VisualLogAnchor.Screen;
		private	GameObject		anchorObj			= null;


		/// <summary>
		/// [GET] Returns whether this log entry is to be considered alive. Dead entries will
		/// be removed immediately.
		/// </summary>
		public bool IsAlive
		{
			get { return this.LifetimeRatio > 0.0f; }
		}
		/// <summary>
		/// [GET / SET] The remaining lifetime of this log entry in milliseconds. If not specified otherwise,
		/// a log entry is only displayed for a single frame.
		/// </summary>
		public float Lifetime
		{
			get { return this.lifetime; }
			set
			{
				this.lifetime = value;
				this.maxLifetime = MathF.Max(this.lifetime, this.maxLifetime);
			}
		}
		/// <summary>
		/// [GET] The relative amount of available lifetime this log entry has. 
		/// 0.0f means "totally dead", 1.0f means "as alive as it gets".
		/// </summary>
		public virtual float LifetimeRatio
		{
			get { return MathF.Clamp(this.lifetime / this.maxLifetime, 0.0f, 1.0f); }
		}
		/// <summary>
		/// [GET / SET] The log entries individual color.
		/// </summary>
		public ColorRgba Color
		{
			get { return this.color; }
			set { this.color = value; }
		}
		/// <summary>
		/// [GET / SET] Whether the lifetime of this entry should be used as alpha-value of the specified color.
		/// </summary>
		public bool LifetimeAsAlpha
		{
			get { return this.lifetimeAsAlpha; }
			set { this.lifetimeAsAlpha = value; }
		}
		/// <summary>
		/// [GET / SET] The anchor which is used for interpreting this log entries coordinates and sizes.
		/// </summary>
		public VisualLogAnchor Anchor
		{
			get { return this.anchor; }
			set
			{
				this.anchor = value;
				if (this.anchor == VisualLogAnchor.Object && this.anchorObj == null)
					this.anchor = VisualLogAnchor.World;
				else
					this.anchorObj = null;
			}
		}
		/// <summary>
		/// [GET / SET] The GameObject to which this log entry is anchored.
		/// </summary>
		public GameObject AnchorObj
		{
			get { return this.anchorObj; }
			set
			{
				this.anchorObj = value;
				this.anchor = this.anchorObj != null ? VisualLogAnchor.Object : VisualLogAnchor.World;
			}
		}


		/// <summary>
		/// Removes this log entry by killing it immediately.
		/// </summary>
		public void Remove()
		{
			this.lifetime = 0.0f;
		}
		/// <summary>
		/// Updates the entry. This method is automatically called by the system.
		/// </summary>
		public virtual void Update()
		{
			this.lifetime -= Time.TimeMult * Time.MsPFMult;
			if (this.anchor == VisualLogAnchor.Object)
			{
				if (this.anchorObj == null || this.anchorObj.Disposed)
				{
					this.Remove();
				}
			}
		}
		/// <summary>
		/// Draws the entry to the specified <see cref="Canvas"/>.
		/// </summary>
		/// <param name="target"></param>
		public void Draw(Canvas target)
		{
			this.Draw(target, Vector3.Zero, 0.0f, 1.0f);
		}
		/// <summary>
		/// Draws the entry to the specified <see cref="Canvas"/>. Its parameters provide information
		/// about the log entries anchor and should be interpreted as parent transform.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="basePos">The anchors base position.</param>
		/// <param name="baseRotation">The anchors base rotation.</param>
		/// <param name="baseScale">The anchors base scale.</param>
		public abstract void Draw(Canvas target, Vector3 basePos, float baseRotation, float baseScale);
	}
}
