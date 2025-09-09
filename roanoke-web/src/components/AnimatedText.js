import React, { useEffect, useRef } from 'react';
import { gsap } from 'gsap';

// Register the SplitText plugin
// Note: SplitText is a premium GSAP plugin - you'll need a Club GreenSock membership
// to use it in production. For development, you can use the trial version.
gsap.registerPlugin(window.SplitText);

const AnimatedText = ({ text, className }) => {
  const textRef = useRef(null);
  const splitTextRef = useRef(null);
  const timelineRef = useRef(null);

  useEffect(() => {
    if (!textRef.current || !window.SplitText) return;

    // Initialize SplitText
    splitTextRef.current = new window.SplitText(textRef.current, {
      type: "words,chars", 
      charsClass: "char",
      wordsClass: "word"
    });
    
    const chars = splitTextRef.current.chars;
    
    // Create animation timeline
    timelineRef.current = gsap.timeline({ 
      repeat: -1,
      repeatDelay: 1
    });
    
    // Initial state - hidden
    gsap.set(chars, { y: 100, autoAlpha: 0 });
    
    // Animate in
    timelineRef.current.to(chars, {
      duration: 0.8, 
      y: 0, 
      autoAlpha: 1, 
      stagger: 0.05,
      ease: "power2.out"
    });
    
    // Hold for 5 seconds
    timelineRef.current.to(chars, { duration: 5 });
    
    // Animate out
    timelineRef.current.to(chars, {
      duration: 0.8, 
      y: -100, 
      autoAlpha: 0, 
      stagger: 0.05,
      ease: "power2.in"
    });
    
    // Cleanup
    return () => {
      if (timelineRef.current) {
        timelineRef.current.kill();
      }
      if (splitTextRef.current) {
        splitTextRef.current.revert();
      }
    };
  }, []);

  return (
    <h1 ref={textRef} className={className}>
      {text}
    </h1>
  );
};

export default AnimatedText;