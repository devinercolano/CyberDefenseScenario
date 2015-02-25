#
# Cookbook Name:: redteam_default.rb
# Recipe:: default
#
# setup the redteam instance for the
# cyber defense scenario

# first download the challenge submission
# utility
remote_file "submit_chall" do
  source "https://raw.githubusercontent.com/clampz/CyberDefenseScenario/master/red_team_submitter/submit_chall"
  path "/usr/sbin/submit_chall"
  mode "0765"
  not_if "test -e /tmp/test-file"
end

# then start the vnc server and create
# a /tmp/test-file so we don't do all
# of this over and over again
script "startVNC" do
  interpreter "bash"
  user "root"
  code <<-EOH
  vnc4server
  touch /tmp/test-file
  EOH
  not_if "test -e /tmp/test-file"
end
