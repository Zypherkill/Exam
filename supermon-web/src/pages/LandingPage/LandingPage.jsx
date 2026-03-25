import './landingpage.css';
import goombaImg from '../../assets/goomba.png';
import pipeImg from '../../assets/pipe.png';

function LandingPage() {
return (
	<div className='landing-page'>
		<h1 className='landing-page__title'>Super Jespermon</h1>
		<canvas className='landing-page__game'></canvas>
		<footer className='landing-page__footer'>
			<img src={pipeImg} alt='pipe' />
			<img className='landing-page__footer__goomba' src={goombaImg} alt='goomba' />
			<img className='landing-page__footer__goomba' src={goombaImg} alt='goomba' />
			<img src={pipeImg} alt='pipe' />
		</footer>
	</div>
);
}

export default LandingPage;