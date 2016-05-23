package smm;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.net.URL;

import javax.xml.stream.events.Comment;

import org.jsoup.Jsoup;
import org.jsoup.nodes.Document;
import org.jsoup.nodes.XmlDeclaration;
import org.jsoup.parser.Parser;
import org.jsoup.select.Elements;

public class SMM {
	static int[] outlinks = new int[10000];
	static int[] comment = new int[10000];
	static int[] article_length = new int[10000];
	static int[] influenceflow = new int [10000];
	static int[] score = new int[10000];
	static int realscore=-2131;
	static String path = "C:\\Users/admin/Desktop/articles/innovp0";
	

	public static void main(String[] args) throws Exception {
		// TODO Auto-generated method stub
		File input = new File(path);
		//URL url = new URL("http://gogoami.pixnet.net/blog/post/158843033");
		//Document xmlDoc =  Jsoup.parse(url, 100000000);
		File[] list = input.listFiles();
		for(int i=0; i < list.length; i++){
			Document xmlDoc =  Jsoup.parse(list[i], "UTF-8"); 
			String a = xmlDoc.toString();
			if(a.contains("input type=\"password\" name=\"encrypted\">"))
				continue;
			else if(a.contains("input name=\"encrypted\" type=\"password\">"))
				continue;
			
			getoutlink(list, i, xmlDoc);
			getcomment(list, i, xmlDoc);
			Elements articlelenth = xmlDoc.select("div[class=\"article-content\"]");
			try{
				article_length[i] = articlelenth.get(0).text().length();
			}catch(Exception e){
				article_length[i] = 40000;
			}
		}
		scoring(list.length);
		System.out.println(realscore);
		
		FileWriter fw = null; 
		BufferedWriter bw = null; 
		try{ 
			fw = new FileWriter(path+"/"+realscore+".txt");
			bw = new BufferedWriter(fw); 
			for(int i = 0; i < list.length ; i++){ 
				bw.write(i + "    " + score[i] + "\r\n"); 
			} 
		} 
		catch(IOException e){} 
		finally{
			try{ 
				bw.close(); 
			} 
		catch(IOException e){} 
		} 
	}
	
	public static void getoutlink(File[] list, int i, Document xmlDoc) {
		try{
			Elements outlink = xmlDoc.select("div[class=\"article-content-inner\"] a");
			Elements image = xmlDoc.select("div[class=\"article-content-inner\"] a img");
			Elements tag = xmlDoc.select("div[class=\"article-content-inner\"] a[rel=\"tag\"]");
			outlinks[i] = outlink.size()-image.size()-tag.size();
		}catch(Exception e){
			outlinks[i]=0;
		}
	}
	
	public static void getcomment(File[] list, int i, Document xmlDoc) {
		try{
			Elements comm = xmlDoc.select("ul[class=\"single-post\"]");
			Elements comm_secret = xmlDoc.select("ul[class=\"single-post secret\"]");
			comment[i] = comm.size()+ comm_secret.size();
		}catch(Exception e){
			comment[i]=0;
		}
	}
	
	public static void scoring(int length) {
		for(int i=0;i<length;i++){
			influenceflow[i]= 1-outlinks[i];
			score[i]=comment[i]+influenceflow[i]+article_length[i];
			System.out.println(score[i]);
		}
	
		for(int i=0;i<length;i++){
			if(score[i]>realscore){
				realscore=score[i];
			}
		}
	}
}
