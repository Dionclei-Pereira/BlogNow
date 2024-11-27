        const email = window.email;
        document.addEventListener('DOMContentLoaded', () => {
        let hearts = document.querySelectorAll('.heart, .heart-liked');
            hearts.forEach(heart => {
        heart.addEventListener('click', () => {
            let postId = heart.dataset.postid;
            const params = new URLSearchParams;
            params.append('PostId', postId);
            params.append('Email', email);
            fetch('/Main/LikePost', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: params.toString()
            })
                .then(response => response.json())
                .then(response => {
                    if (response.error) {
                        alert(response.error)
                        return;
                    }
                    const postElement = document.querySelector(`[data-postid="${postId}"]`);
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