const email = window.email;
document.addEventListener('DOMContentLoaded', () => {
    let hearts = document.querySelectorAll('.heart, .heart-liked');
    hearts.forEach(heart => {
        heart.addEventListener('click', (event) => {
            event.stopPropagation();
            let postId = heart.dataset.postid;
            const params = new URLSearchParams;
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;
            params.append('postId', postId);
            params.append('email', email);
            fetch('/Post/LikePost', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': token,
                },
                body: params.toString()
            })
                .then(response => response.json())
                .then(response => {
                    if (response.error) {
                        alert(response.error)
                        return;
                    }
                    const postElement = document.querySelector(`button[data-postid="${postId}"]`);
                    postElement.className = response.status;
                    const likesCount = document.querySelector(`#likes-count-${postId}`);
                    if (likesCount) {
                        likesCount.textContent = response.likes;
                    }
                }).catch(() => {
                    alert('Error');
                })
        })
    })
})