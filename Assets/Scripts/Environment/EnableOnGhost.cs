using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Events;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Environment
{
    public class EnableOnGhost : BaseGameObject
    {
        public SpriteRenderer Renderer;
        protected override void OnEnable()
        {
            base.OnEnable();
            this.ObserveEvent(StateEvents.OnGhostForm, OnEvent);
        }

        private void OnEvent()
        {
            Renderer.enabled = true;
            this.StopObservingEvent(StateEvents.OnGhostForm, OnEvent);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.StopObservingEvent(StateEvents.OnGhostForm, OnEvent);
        }
    }
}
