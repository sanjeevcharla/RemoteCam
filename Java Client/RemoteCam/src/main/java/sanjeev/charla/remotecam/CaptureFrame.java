package sanjeev.charla.remotecam;

import java.awt.BorderLayout;
import java.awt.Container;
import java.awt.FlowLayout;
import java.awt.Font;
import java.awt.Image;
import java.awt.Label;
import java.awt.image.BufferedImage;
import java.io.BufferedReader;
import java.io.ByteArrayInputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

import javax.imageio.ImageIO;
import javax.swing.BoxLayout;
import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.xml.bind.DatatypeConverter;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;
import com.microsoft.signalr.HubConnection;
import com.microsoft.signalr.HubConnectionBuilder;

public class CaptureFrame extends JFrame {

	private static final long serialVersionUID = 1L;
	private static final String hostName = "http://10.0.0.3:6789/";
	private static final String hubUrl = hostName + "cam";
	private static final String captureUrl = hostName + "api/cam";
	private static Gson gson;

	private JLabel timeStampLabel, imageLabel, titleLabel;

	static {
		gson = new GsonBuilder().create();
	}

	public CaptureFrame() {
		setTitle("Remote Cam");
		setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		configureUI();
		pack();
		setSize(640, 480);
		setLocationRelativeTo(null);
		setVisible(true);

		listenToHub();
	}

	private void configureUI() {
		Container container = getContentPane();

		JPanel infoPanel = new JPanel();
		infoPanel.setLayout(new BoxLayout(infoPanel, BoxLayout.Y_AXIS));
		titleLabel = new JLabel("Remote Cam");
		titleLabel.setFont(new Font("Segoe UI Light", Font.PLAIN, 25));
		titleLabel.setAlignmentX(CENTER_ALIGNMENT);
		infoPanel.add(titleLabel);

		JPanel timePanel = new JPanel();
		timePanel.setLayout(new FlowLayout());
		timePanel.add(new JLabel("Last Capture Time:"));
		timePanel.add(timeStampLabel = new JLabel());

		infoPanel.add(timePanel);

		container.add(infoPanel, BorderLayout.PAGE_START);
		container.add(imageLabel = new JLabel(), BorderLayout.CENTER);
	}

	private void listenToHub() {
		HubConnection hubConnection = HubConnectionBuilder.create(hubUrl).build();
		hubConnection.start();
		hubConnection.on("capture", () -> {
			System.out.println("Capture update from Hub");
			getCapture();
		});
	}

	private void getCapture() {
		HttpURLConnection con;
		try {
			URL url = new URL(captureUrl);
			con = (HttpURLConnection) url.openConnection();
			con.setRequestMethod("GET");
			con.connect();
			if (con.getResponseCode() == 200) {
				StringBuilder sb = new StringBuilder();
				String line;
				BufferedReader rd = new BufferedReader(new InputStreamReader(con.getInputStream()));
				while ((line = rd.readLine()) != null)
					sb.append(line);
				rd.close();
				Capture capture = gson.fromJson(sb.toString(), Capture.class);
				if (capture != null) {
					byte[] imageBytes = DatatypeConverter.parseBase64Binary(capture.getImage64());
					Image image = ImageIO.read(new ByteArrayInputStream(imageBytes))
							.getScaledInstance(imageLabel.getWidth(), imageLabel.getHeight(), Image.SCALE_SMOOTH);
					timeStampLabel.setText(capture.getTimeStamp());
					imageLabel.setIcon(new ImageIcon(image));
				} else
					System.out.println("Capture not found: " + sb);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

}