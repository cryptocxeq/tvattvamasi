using DG.Tweening;

public static class Utility 
{

    public static Sequence NewSequence()
    {
        Sequence seq = DOTween.Sequence();
        seq.Pause();
        return seq;
    }

}
