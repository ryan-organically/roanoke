import React, { useEffect } from 'react';
import AnimatedText from './AnimatedText';
import { gsap } from 'gsap';

const LandingPage = ({ onExplore }) => {
  useEffect(() => {
    // Add the SplitText script from the CDN
    // This is a workaround since SplitText is a premium plugin
    const script = document.createElement('script');
    script.src = 'https://s3-us-west-2.amazonaws.com/s.cdpn.io/16327/SplitText3.min.js';
    script.async = true;
    document.body.appendChild(script);

    return () => {
      if (document.body.contains(script)) {
        document.body.removeChild(script);
      }
    };
  }, []);

  const handleExploreClick = (e) => {
    e.stopPropagation(); // Prevent the click from bubbling up
    if (typeof window !== 'undefined') {
      // Add a click animation using GSAP
      gsap.to(e.currentTarget, {
        scale: 0.95,
        duration: 0.1,
        onComplete: () => {
          gsap.to(e.currentTarget, {
            scale: 1,
            duration: 0.1,
            onComplete: () => {
              // Call the parent's onExplore handler after animation completes
              if (onExplore) {
                onExplore();
              }
            }
          });
        }
      });
    }
  };

  return (
    <div className="landing-page">
      <div className="hero-section">
        <AnimatedText 
          text="WELCOME TO ROANOKE" 
          className="hero-title"
        />
        <p className="hero-subtitle">
          A Sci-fi Adventure in the 16th Century
        </p>
        <div className="cta-buttons">
          <button 
            className="primary-button"
            onClick={handleExploreClick}
          >
            Explore Marketplace
          </button>
          <button 
            className="secondary-button"
            onClick={handleExploreClick}
          >
            Learn More
          </button>
        </div>
      </div>
    </div>
  );
};

export default LandingPage;