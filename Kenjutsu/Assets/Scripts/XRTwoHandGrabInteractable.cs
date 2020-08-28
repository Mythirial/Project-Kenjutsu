using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class XRTwoHandGrabInteractable : XRGrabInteractable
    {
        public enum TwoHandRotationType
        {
            None,
            First,
            Second
        };

        public TwoHandRotationType twoHandRotationType;
        public bool snapToSecondHand = true;
        public List<XRSimpleInteractable> secondHandInteractables = new List<XRSimpleInteractable>();
        private XRBaseInteractor _secondInteractor;
        private Quaternion _initialAttachRotation;
        private Quaternion _initialRotationOffset;

        // Start is called before the first frame update
        private void Start()
        {
            foreach (var interactable in secondHandInteractables)
            {
                interactable.onSelectEnter.AddListener(OnSecondHandGrab);
                interactable.onSelectExit.AddListener(OnSecondHandRelease);
            }
        }

        private Quaternion GetTwoHandRotation()
        {
            Quaternion targetRotation;
            if(twoHandRotationType == TwoHandRotationType.None)
                targetRotation = Quaternion.LookRotation(_secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position);
            else if(twoHandRotationType == TwoHandRotationType.First)
                targetRotation = Quaternion.LookRotation(_secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, selectingInteractor.attachTransform.up);
            else
                targetRotation = Quaternion.LookRotation(_secondInteractor.attachTransform.position - selectingInteractor.attachTransform.position, _secondInteractor.attachTransform.up);

            return targetRotation;
        }

        public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
        {
            if (_secondInteractor && selectingInteractor)
            {
                //compute rotation
                if (snapToSecondHand)
                    selectingInteractor.attachTransform.rotation = GetTwoHandRotation();
                else
                    selectingInteractor.attachTransform.rotation = GetTwoHandRotation() * _initialRotationOffset;
            }
            base.ProcessInteractable(updatePhase);
        }

        public void OnSecondHandGrab(XRBaseInteractor interactor)
        {
            _secondInteractor = interactor;
            _initialRotationOffset = Quaternion.Inverse(GetTwoHandRotation()) * selectingInteractor.attachTransform.rotation;

        }
        public void OnSecondHandRelease(XRBaseInteractor interactor)
        {
            _secondInteractor = null;
        }

        protected override void OnSelectEnter(XRBaseInteractor interactor)
        {
            base.OnSelectEnter(interactor);
            _initialAttachRotation = interactor.attachTransform.localRotation;
        }

        protected override void OnSelectExit(XRBaseInteractor interactor)
        {
            base.OnSelectExit(interactor);
            _secondInteractor = null;
            interactor.attachTransform.localRotation = _initialAttachRotation;
        }

        public override bool IsSelectableBy(XRBaseInteractor interactor)
        {
            bool isAlreadyGrabbed = selectingInteractor && !interactor.Equals(selectingInteractor);
            return base.IsSelectableBy(interactor) && !isAlreadyGrabbed;
        }
    }
}