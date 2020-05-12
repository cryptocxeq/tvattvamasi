using UnityEngine;
using DG.Tweening;

public class MandalaController : MonoBehaviour
{
	public ParticleSystem innercircleps;
	public ParticleSystem outercircleps;
	public ParticleSystem innerparsys;
	public ParticleSystem outerparsys;
	public ParticleSystem innerdissolveps;
	public ParticleSystem outerdissolveps;
	public ParticleSystem ambient1;
	public ParticleSystem ambient2;
	public Material innercirclemat;
	public Material outercirclemat;
	public GameObject mdla;

	public ParticleSystemRenderer[] particleRenderers
	{
		get; private set;
	}

	public System.Action onDissolveEnd = () => { };

	private void Awake()
	{
		particleRenderers = GetComponentsInChildren<ParticleSystemRenderer>(true);
	}

	void OnEnable()
	{
		innercirclemat.DOFloat(0f, "_DissolveAmount", 0.5f);
		outercirclemat.DOFloat(0f, "_DissolveAmount", 0.5f);
	}

	void Update()
	{
		ParticleSystem.Particle[] ipx = new ParticleSystem.Particle[innercircleps.particleCount];
		innercircleps.GetComponent<ParticleSystem>().GetParticles(ipx);


		ParticleSystem.Particle[] opx = new ParticleSystem.Particle[outercircleps.particleCount];
		outercircleps.GetComponent<ParticleSystem>().GetParticles(opx);
		//innerspr.transform.eulerAngles = new Vector3(0, 0, -opx[1].rotation);
		var ish = outerdissolveps.shape;
		Vector3 iv3 = new Vector3(0f, 0f, ipx[1].rotation);
		ish.rotation = iv3;

		var osh = outerdissolveps.shape;
		Vector3 ov3 = new Vector3(0f, 0f, opx[1].rotation);
		osh.rotation = ov3;

		// Debug.Log("ipx0 " + ipx[0].rotation + " ipx1 " + ipx[1].rotation);

		//        Debug.Log("opx0 " + opx[0].rotation + " opx1 " + opx[1].rotation);
		// Debug.Log("spr " + innerspr.transform.rotation);
		//  }

	}

	public void Dissolve()
	{

		var ama = ambient1.main;
		var aem = ambient1.emission;

		var a2em = ambient2.emission;
		var a2ma = ambient2.main;

		var ano = ambient1.noise;
		var ave = ambient1.velocityOverLifetime.radial.constant;

		innercirclemat.SetFloat("_DissolveAmount", 0);
		outercirclemat.SetFloat("_DissolveAmount", 0);
		outerdissolveps.Emit(4500);
		innerdissolveps.Emit(1500);

		Sequence seq = Utility.NewSequence();
		Sequence dseq = Utility.NewSequence();

		seq.Append(DOTween.To(Ambient1Emissions, 100, 0, 6))
			.Join(DOTween.To(Ambient2Emissions, 100, 0, 6))
			.Join(DOTween.To(AmbientRadialize, 0f, 0.4f, 6))
			.Join(DOTween.To(AmbientTransparency, 1f, 0f, 6))
			.Join(DOTween.To(TrailAlpha, 1f, 0f, 6))
			.Join(DOTween.To(ParticlesNoise, 0f, 0.15f, 8))
			.Join(DOTween.To(ParticlesRadialize, 0f, 0.15f, 8))
			.Join(DOTween.To(OuterParticlesEmission, 200, 0, 4))
			.Join(DOTween.To(InnerParticlesEmission, 100, 0, 4))
			.AppendCallback(FinishDissolve);

		dseq.Append(innercirclemat.DOFloat(0.25f, "_DissolveAmount", 0.5f))
			.Join(outercirclemat.DOFloat(0.25f, "_DissolveAmount", 0.5f))
			.Append(innercirclemat.DOFloat(0.5f, "_DissolveAmount", 0.75f))
			.Join(outercirclemat.DOFloat(0.5f, "_DissolveAmount", 0.75f))
			.Append(innercirclemat.DOFloat(0.75f, "_DissolveAmount", 1f))
			.Join(outercirclemat.DOFloat(0.75f, "_DissolveAmount", 1f))
			.Append(innercirclemat.DOFloat(1f, "_DissolveAmount", 1.5f))
			.Join(outercirclemat.DOFloat(1f, "_DissolveAmount", 1.5f))
			.AppendCallback(OnDissolveEnd);

		seq.Play();
		dseq.Play();
	}

	void OnDissolveEnd()
	{
		onDissolveEnd.Invoke();
	}

	void AmbientRadialize(float xf)
	{
		var ave = ambient1.velocityOverLifetime;

		var rad1 = ave.radial;
		rad1.mode = ParticleSystemCurveMode.Constant;
		rad1.constant = xf;
		ave.radial = rad1;

		var a2ve = ambient2.velocityOverLifetime;
		var rad2 = a2ve.radial;
		rad2.mode = ParticleSystemCurveMode.Constant;
		rad2.constant = xf;
		a2ve.radial = rad2;
	}

	void TrailAlpha(float tf)
	{
		var opst = outerparsys.trails;
		Color tc = opst.colorOverLifetime.color;
		tc.a = tf;
		opst.colorOverLifetime = tc;
	}
	void Ambient1Emissions(float ef)
	{
		var aem = ambient1.emission;
		aem.rateOverTime = ef;
	}
	void Ambient2Emissions(float ef)
	{
		var a2em = ambient2.emission;
		a2em.rateOverTime = ef;
	}

	void AmbientTransparency(float af)
	{
		var a2ma = ambient2.main;
		Color a2c = a2ma.startColor.color;
		a2c.a = af;

		a2ma.startColor = a2c;
	}

	void ParticlesNoise(float nf)
	{
		var ipsn = innerparsys.noise;
		var opsn = outerparsys.noise;
		ipsn.enabled = true;
		opsn.enabled = true;
		opsn.strength = nf;
		ipsn.strength = nf;
	}

	void ParticlesRadialize(float pf)
	{
		var ipsve = innerparsys.velocityOverLifetime;

		var rad1 = ipsve.radial;
		rad1.mode = ParticleSystemCurveMode.Constant;
		rad1.constant = pf;
		ipsve.radial = rad1;

		var opsve = outerparsys.velocityOverLifetime;
		var rad2 = opsve.radial;
		rad2.mode = ParticleSystemCurveMode.Constant;
		rad2.constant = pf;
		opsve.radial = rad2;
	}

	void OuterParticlesEmission(float ef)
	{
		var opem = outerparsys.emission;
		opem.rateOverTime = ef;
	}

	void InnerParticlesEmission(float ef)
	{
		var ipem = innerparsys.emission;
		ipem.rateOverTime = ef;
	}

	void FinishDissolve()
	{
		Debug.Log("Finishing dissolve");
		var aem = ambient1.emission;
		var a2em = ambient2.emission;
		aem.enabled = false;
		a2em.enabled = false;

		var opem = outerparsys.emission;
		var ipem = innerparsys.emission;
		opem.enabled = false;
		ipem.enabled = false;


		var icem = innercircleps.emission;
		var ocem = outercircleps.emission;

		icem.enabled = false;
		ocem.enabled = false;
		Destroy(mdla);
	}
}
