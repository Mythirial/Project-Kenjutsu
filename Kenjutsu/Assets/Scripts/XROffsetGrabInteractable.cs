using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Assets.Scripts
{
    public class XROffsetGrabInteractable : XRGrabInteractable
    {
        private Vector3 _initialAttachmentLocalPos;
        private Quaternion _initialAttachmentLocalRot;

        // Start is called before the first frame update
        private void Start()
        {
            //Create Attachment point
            if (!attachTransform)
            {
                GameObject grab = new GameObject("Grab Pivot");
                grab.transform.SetParent(transform, false);
                attachTransform = grab.transform;
            }

            _initialAttachmentLocalPos = attachTransform.localPosition;
            _initialAttachmentLocalRot = attachTransform.localRotation;
        }

        protected override void OnSelectEnter(XRBaseInteractor interactor)
        {
            if (interactor is XRDirectInteractor)
            {
                attachTransform.position = interactor.transform.position;
                attachTransform.rotation = interactor.transform.rotation;
            }
            else
            {
                attachTransform.localPosition = _initialAttachmentLocalPos;
                attachTransform.localRotation = _initialAttachmentLocalRot;
            }

            base.OnSelectEnter(interactor);
        }
    }
}
