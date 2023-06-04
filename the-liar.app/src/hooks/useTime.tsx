import { useEffect, useState } from "react";


const useTime = (refreshPeriod: number = 1000) => {
    const [currentTime, setCurrentTime] = useState<Date>();

    useEffect(() => {
        const interval = setInterval(
            () => setCurrentTime(new Date()),
            refreshPeriod
        );
        return () => clearInterval(interval);
    }, [refreshPeriod]);

    return currentTime;
};


export default useTime;