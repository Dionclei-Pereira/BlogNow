document.addEventListener('DOMContentLoaded', () => {
   document.addEventListener("click", (event) => {
        if (event.target.classList.contains("close-btn")) {
            const modalPost = event.target.closest(".modal-post");
            if (modalPost) {
                modalPost.remove();
            }
        }
   });
    const posts = document.querySelectorAll('.Comment');
    posts.forEach(post => {
        post.addEventListener('click', () => {
            const postId = post.dataset.postid;
            const params = new URLSearchParams;
            params.append('PostId', postId);
            fetch('/Main/PostReturn', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
                body: params.toString()
            })
                .then(response => response.text())
                .then(response => {
                    if (response.error) {
                        alert(response.error)
                        return;
                    }
                    document.querySelector(".container-post").innerHTML = response;
                }).catch(() => {
                    alert('Error');
                })
        })
    })
})