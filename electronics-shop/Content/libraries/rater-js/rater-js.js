//const allStar = document.querySelectorAll('.rating .star');
//const ratingValue = document.querySelector('.rating input');

//allStar.forEach((item, idx) => {
//	item.addEventListener('click', function () {
//		let click = 0;
//		ratingValue.value = idx + 1;

//		allStar.forEach(i => {
//			i.classList.replace('bi-star-fill', 'bi-star')
//			i.classList.remove('active')
//		});

//		for (let i = 0; i < allStar.length; i++) {
//			if (i <= idx) {
//				allStar[i].classList.replace('bi-star', 'bi-star-fill')
//				allStar[i].classList.add('active')
//			} else {
//				allStar[i].style.setProperty('--i', click)
//				click++
//			}
//		}
//	});
//});

const allStar = document.querySelectorAll('.rating .star');
const ratingValue = document.querySelector('.rating input');
// Bi?n ?? theo d�i vi?c ch?n sao
let isStarSelected = false;

allStar.forEach((item, idx) => {
	item.addEventListener('click', function () {
		let click = 0;
		ratingValue.value = idx + 1;

		allStar.forEach(i => {
			i.classList.replace('bi-star-fill', 'bi-star')
			i.classList.remove('active')
		});

		for (let i = 0; i < allStar.length; i++) {
			if (i <= idx) {
				allStar[i].classList.replace('bi-star', 'bi-star-fill')
				allStar[i].classList.add('active')
			} else {
				allStar[i].style.setProperty('--i', click)
				click++
			}
		}
		// Ki?m tra xem c� �t nh?t m?t sao ???c ch?n sau m?i l?n click
		isStarSelected = true;
	});
});



// X? l� khi ng??i d�ng g?i x?p h?ng
// V� d?: S? d?ng m?t n�t ho?c form
submitButton.addEventListener('click', function (event) {
	event.preventDefault(); // Ng?n ch?n h�nh ??ng m?c ??nh c?a n�t ho?c form

	if (!isStarSelected) {
		// N?u kh�ng c� sao n�o ???c ch?n, th�ng b�o cho ng??i d�ng v� ng?n ng??i d�ng g?i x?p h?ng
		alert('Please choose at least one star!');
		return; // Ng?n ch?n vi?c g?i x?p h?ng n?u kh�ng c� sao n�o ???c ch?n
	}

	// N?u c� �t nh?t m?t sao ???c ch?n, ti?n h�nh g?i x?p h?ng
	// Th?c hi?n h�nh ??ng g?i x?p h?ng ? ?�y
	console.log('?� g?i x?p h?ng!');
});