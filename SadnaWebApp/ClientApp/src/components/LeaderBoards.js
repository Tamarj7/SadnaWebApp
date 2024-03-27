import React, { useState, useEffect,useMemo } from 'react';
import LeaderBoardCard from "./LeaderBoardCard.js";

const LeaderBoards = () => {

    const [scores, setScores] = useState([]);
    const token = sessionStorage.getItem('token');

    const headers = useMemo(() => {
        return {
            Authorization: `Bearer ${token}`,
            'Content-Type': 'application/json',
        };
    }, [token]);



    useEffect(() => {
        fetch(`/topScoresDB/GetTopScore`, { headers })
            .then((results) => {
                return results.json();
            })
            .then(data => {
                setScores(data);
            })

    }, [])



    return (
        
        
        scores.map((board) => (      //running threw each grid size and generating a leader board card

            <LeaderBoardCard Board={board} />
        ))
    )
} 
export default LeaderBoards;