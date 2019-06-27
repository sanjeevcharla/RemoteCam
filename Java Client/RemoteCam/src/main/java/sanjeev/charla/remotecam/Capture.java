package sanjeev.charla.remotecam;

public class Capture {
	private String timeStamp;
	private String image64;

	public String getImage64() {
		return image64;
	}

	public void setImage64(String i) {
		this.image64 = i;
	}

	public String getTimeStamp() {
		return timeStamp;
	}

	public void setTimeStamp(String timeStamp) {
		this.timeStamp = timeStamp;
	}
}