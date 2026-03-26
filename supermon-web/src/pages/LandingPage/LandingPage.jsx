import './landingpage.css';
import goombaImg from '../../assets/goomba.png';
import pipeImg from '../../assets/pipe.png';
import { useEffect, useRef } from 'react';

function LandingPage() {
	const canvasRef = useRef(null);
	const unityInstanceRef = useRef(null);

	const handleFullscreen = () => {
			unityInstanceRef.current.SetFullscreen(1);
	};

	useEffect(() => {
		const canvas = canvasRef.current;
		if (!canvas) return;

		// Load Unity Build
		const buildUrl = '/Build/Build';
		const loaderUrl = buildUrl + '/Build.loader.js';
		const config = {
			arguments: [],
			dataUrl: buildUrl + '/Build.data',
			frameworkUrl: buildUrl + '/Build.framework.js',
			codeUrl: buildUrl + '/Build.wasm',
			streamingAssetsUrl: '/Build/StreamingAssets',
			companyName: 'NikkiSmells',
			productName: 'SuperJesperMon',
			productVersion: '1.0',
			matchWebGLToCanvasSize: true,
			canvas: canvas,
		};

		// Function to initialize Unity when loader is ready
		const initializeUnity = () => {
			if (typeof createUnityInstance === 'undefined') {
				setTimeout(initializeUnity, 100);
				return;
			}
			createUnityInstance(canvas, config, () => {
			})
				.then((unityInstance) => {
					unityInstanceRef.current = unityInstance;
				})
				.catch((message) => {
					message = message.toString();
					console.error(message);
				});
		};

		// Load the Unity loader script
		const script = document.createElement('script');
		script.src = loaderUrl;
		script.onload = () => {
			initializeUnity();
		};
		script.onerror = () => {
			// Failed to load Unity loader script
			console.error('Failed to load Unity loader script');
		};
		document.head.appendChild(script);

		return () => {
			// Cleanup script on unmount if needed
			if (script.parentNode) {
				document.head.removeChild(script);
			}
		};
	}, []);

	return (
		<div className='landing-page'>
			<h1 className='landing-page__title'>Super Jespermon</h1>
			<div
				className='landing-page__game-container'
				style={{ position: 'relative' }}>
				<canvas
					onClick={handleFullscreen}
					ref={canvasRef}
					id='unity-canvas'
					className='landing-page__canvas'
					width='960'
					height='600'></canvas>
			</div>
			<footer className='landing-page__footer'>
				<img className='landing-page__footer--pipe' src={pipeImg} alt='pipe' />
				<img
					className='landing-page__footer--goomba'
					src={goombaImg}
					alt='goomba'
				/>
				<img
					className='landing-page__footer--goomba'
					src={goombaImg}
					alt='goomba'
				/>
				<img className='landing-page__footer--pipe' src={pipeImg} alt='pipe' />
			</footer>
		</div>
	);
}

export default LandingPage;
