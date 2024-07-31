export async function copyInvitationLink(link) {
    await navigator.clipboard.writeText(link).then(() => {
        const button = document.getElementById("copy-invitation-link-button");
        if (!button) {
            return;
        }

        button.textContent = "Copied!";
        button.classList.remove("btn-outline-primary");
        button.classList.add("btn-outline-secondary");
    });
}