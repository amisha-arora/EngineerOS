import type { ReactNode } from "react";

type ModalProps = {
    isOpen: boolean;
    title: string;
    children: ReactNode;
    onClose: () => void;
};

export default function Modal({
    isOpen,
    title,
    children,
    onClose,
}: ModalProps) {
    if (!isOpen) {
        return null;
    }

    return (
        <div
            className="modal-overlay"
            onClick={onClose}
        >
            <div
                className="modal"
                role="dialog"
                aria-modal="true"
                aria-labelledby="modal-title"
                onClick={(event) =>
                    event.stopPropagation()
                }
            >
                <div className="modal-header">

                    <h2 id="modal-title">
                        {title}
                    </h2>

                    <button
                        type="button"
                        className="modal-close-button"
                        onClick={onClose}
                        aria-label="Close modal"
                    >
                        ×
                    </button>

                </div>

                <div className="modal-content">
                    {children}
                </div>

            </div>
        </div>
    );
}