﻿namespace DirectorEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using UnityEditor;
    using UnityEngine;

    public abstract class DirectorBehaviourControl
    {
        private UnityEngine.Behaviour behaviour;

        [field: CompilerGenerated]
        public event DirectorBehaviourControlHandler DeleteRequest;

        protected DirectorBehaviourControl()
        {
        }

        internal void Delete()
        {
            Undo.DestroyObjectImmediate(this.Behaviour.gameObject);
        }

        public void RequestDelete()
        {
            if (this.DeleteRequest != null)
            {
                this.DeleteRequest(this, new DirectorBehaviourControlEventArgs(this.behaviour, this));
            }
        }

        public void RequestDelete(DirectorBehaviourControlEventArgs args)
        {
            if (this.DeleteRequest != null)
            {
                this.DeleteRequest(this, args);
            }
        }

        public UnityEngine.Behaviour Behaviour
        {
            get
            {
                return this.behaviour;
            }
            set
            {
                this.behaviour = value;
            }
        }

        public bool IsSelected
        {
            get
            {
                return ((this.Behaviour != null) && Selection.Contains(this.Behaviour.gameObject));
            }
        }
    }
}

